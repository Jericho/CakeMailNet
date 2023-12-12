using CakeMailNet;
using CakeNet.IntegrationTests.Tests;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CakeNet.IntegrationTests
{
	internal class TestsRunner
	{
		private const int MAX_CAKEMAIL_API_CONCURRENCY = 5;
		private const int TEST_NAME_MAX_LENGTH = 25;
		private const string SUCCESSFUL_TEST_MESSAGE = "Completed successfully";

		private enum ResultCodes
		{
			Success = 0,
			Exception = 1,
			Cancelled = 1223
		}

		private readonly ILoggerFactory _loggerFactory;

		public TestsRunner(ILoggerFactory loggerFactory)
		{
			_loggerFactory = loggerFactory;
		}

		public async Task<int> RunAsync()
		{
			// -----------------------------------------------------------------------------
			// Do you want to proxy requests through a tool such as Fiddler? Very useful for debugging.
			var useProxy = true;

			// By default Fiddler Classic uses port 8888 and Fiddler Everywhere uses port 8866
			var proxyPort = 8888;

			// Do you want to log the details of each exception? Can be overwhelming is there are a lot of exceptions.
			var logExceptionDetails = false;
			// -----------------------------------------------------------------------------

			// Configure the proxy if desired
			var proxy = useProxy ? new WebProxy($"http://localhost:{proxyPort}") : null;

			// configure the CakeNet client
			var connectionInfo = GetConnectionInfo();
			var client = new CakeMailNetClient(connectionInfo, proxy, null, logExceptionDetails ? _loggerFactory.CreateLogger<CakeMailNetClient>() : null);

			// Configure Console
			var source = new CancellationTokenSource();
			Console.CancelKeyPress += (s, e) =>
			{
				e.Cancel = true;
				source.Cancel();
			};

			// Ensure the Console is tall enough and centered on the screen
			if (OperatingSystem.IsWindows()) Console.WindowHeight = Math.Min(60, Console.LargestWindowHeight);
			ConsoleUtils.CenterConsole();

			// These are the integration tests that we will execute
			IEnumerable<Type> integrationTests =
			[
				typeof(ContactsAndLists),
			];

			// Execute the async tests in parallel (with max degree of parallelism)
			var results = await integrationTests.ForEachAsync(
				async testType =>
				{
					var log = new StringWriter();

					try
					{
						var integrationTest = (IIntegrationTest)Activator.CreateInstance(testType);
						await integrationTest.RunAsync(client, log, source.Token).ConfigureAwait(false);
						return (TestName: testType.Name, ResultCode: ResultCodes.Success, Message: SUCCESSFUL_TEST_MESSAGE);
					}
					catch (OperationCanceledException)
					{
						await log.WriteLineAsync($"-----> TASK CANCELLED").ConfigureAwait(false);
						return (TestName: testType.Name, ResultCode: ResultCodes.Cancelled, Message: "Task cancelled");
					}
					catch (Exception e)
					{
						var exceptionMessage = e.GetBaseException().Message;
						await log.WriteLineAsync($"-----> AN EXCEPTION OCCURRED: {exceptionMessage}").ConfigureAwait(false);
						return (TestName: testType.Name, ResultCode: ResultCodes.Exception, Message: exceptionMessage);
					}
					finally
					{
						lock (Console.Out)
						{
							Console.Out.WriteLine(log.ToString());
						}
					}
				}, MAX_CAKEMAIL_API_CONCURRENCY)
			.ConfigureAwait(false);

			// Display summary
			var summary = new StringWriter();
			await summary.WriteLineAsync("\n\n**************************************************").ConfigureAwait(false);
			await summary.WriteLineAsync("******************** SUMMARY *********************").ConfigureAwait(false);
			await summary.WriteLineAsync("**************************************************").ConfigureAwait(false);

			var nameMaxLength = Math.Min(results.Max(r => r.TestName.Length), TEST_NAME_MAX_LENGTH);
			foreach (var (TestName, ResultCode, Message) in results.OrderBy(r => r.TestName).ToArray())
			{
				await summary.WriteLineAsync($"{TestName.ToExactLength(nameMaxLength)} : {Message}").ConfigureAwait(false);
			}

			await summary.WriteLineAsync("**************************************************").ConfigureAwait(false);
			await Console.Out.WriteLineAsync(summary.ToString()).ConfigureAwait(false);

			// Prompt user to press a key in order to allow reading the log in the console
			var promptLog = new StringWriter();
			await promptLog.WriteLineAsync("\n\n**************************************************").ConfigureAwait(false);
			await promptLog.WriteLineAsync("Press any key to exit").ConfigureAwait(false);
			ConsoleUtils.Prompt(promptLog.ToString());

			// Return code indicating success/failure
			var resultCode = (int)ResultCodes.Success;
			if (results.Any(result => result.ResultCode != ResultCodes.Success))
			{
				if (results.Any(result => result.ResultCode == ResultCodes.Exception)) resultCode = (int)ResultCodes.Exception;
				else if (results.Any(result => result.ResultCode == ResultCodes.Cancelled)) resultCode = (int)ResultCodes.Cancelled;
				else resultCode = (int)results.First(result => result.ResultCode != ResultCodes.Success).ResultCode;
			}

			return await Task.FromResult(resultCode);
		}

		private static IConnectionInfo GetConnectionInfo()
		{
			// OAuth
			var userName = Environment.GetEnvironmentVariable("CAKEMAIL_USERNAME", EnvironmentVariableTarget.User);
			var password = Environment.GetEnvironmentVariable("CAKEMAIL_PASSWORD", EnvironmentVariableTarget.User);
			var accessToken = Environment.GetEnvironmentVariable("CAKEMAIL_OAUTH_ACCESSTOKEN", EnvironmentVariableTarget.User);
			var refreshToken = Environment.GetEnvironmentVariable("CAKEMAIL_OAUTH_REFRESHTOKEN", EnvironmentVariableTarget.User);

			if (string.IsNullOrEmpty(userName)) throw new Exception("You must set the CAKEMAIL_USERNAME environment variable before you can run integration tests.");
			if (string.IsNullOrEmpty(password)) throw new Exception("You must set the CAKEMAIL_PASSWORD environment variable before you can run integration tests.");

			return OAuthConnectionInfo.WithUserCredentials(userName, password, accessToken, refreshToken,
				(newRefreshToken, newAccessToken) =>
				{
					Environment.SetEnvironmentVariable("CAKEMAIL_OAUTH_ACCESSTOKEN", newAccessToken, EnvironmentVariableTarget.User);
					Environment.SetEnvironmentVariable("CAKEMAIL_OAUTH_REFRESHTOKEN", newRefreshToken, EnvironmentVariableTarget.User);
				});
		}
	}
}
