using System.Net;
using System.Threading.Tasks;
using Clockify.Net;
using Clockify.Net.Models.Workspaces;
using FluentAssertions;
using NUnit.Framework;

namespace Clockify.Tests {
	public class UserTests {
		private readonly ClockifyClient _client;
		private string _workspaceId;
		public UserTests() {
			_client = new ClockifyClient();
		}

		[SetUp]
		public async Task Setup() {
			var result = await _client.CreateWorkspaceAsync(new WorkspaceRequest { Name = "UserTestWorkspace" });
			result.IsSuccessful.Should().BeTrue();
			_workspaceId = result.Data.Id;
		}

		[TearDown]
		public async Task Cleanup() {
			var result = await _client.DeleteWorkspaceAsync(_workspaceId);
			result.IsSuccessful.Should().BeTrue();
		}

		[Test]
		public async Task GetCurrentUser_ShouldReturnCurrentUser() {
			var response = await _client.GetCurrentUserAsync();
			response.IsSuccessful.Should().BeTrue();
			response.Data.Should().NotBeNull();
		}

		[Test]
		public async Task GetCurrentUser_WrongApiKey_ShouldReturnCurrentUser() {
			var response = await new ClockifyClient("wrong_key").GetCurrentUserAsync();
			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Test]
		public async Task FindAllUsersOnWorkspace_GoodWorkspace_ShouldReturnCurrentUser() {
			var response = await _client.FindAllUsersOnWorkspaceAsync(_workspaceId);
			response.IsSuccessful.Should().BeTrue();
			response.Data.Should().NotBeNullOrEmpty();
		}
	}
}