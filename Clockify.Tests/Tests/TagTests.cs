﻿using System;
using System.Threading.Tasks;
using Clockify.Net;
using Clockify.Net.Models.Tags;
using Clockify.Net.Models.Workspaces;
using Clockify.Tests.Fixtures;
using FluentAssertions;
using NUnit.Framework;

namespace Clockify.Tests.Tests
{
    public class TagTests
    {
        private readonly ClockifyClient _client;
        private string _workspaceId;

        public TagTests()
        {
            _client = new ClockifyClient();
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var workspaceResponse = await _client.CreateWorkspaceAsync(new WorkspaceRequest {Name = "TagsWorkspace"});
            workspaceResponse.IsSuccessful.Should().BeTrue();
            _workspaceId = workspaceResponse.Data.Id;
        }

        [OneTimeTearDown]
        public async Task Cleanup()
        {
	        var currentUser = await _client.GetCurrentUserAsync();
	        var changeResponse =
		        await _client.SetActiveWorkspaceFor(currentUser.Data.Id, DefaultWorkspaceFixture.DefaultWorkspaceId);
	        changeResponse.IsSuccessful.Should().BeTrue();
            var workspaceResponse = await _client.DeleteWorkspaceAsync(_workspaceId);
            workspaceResponse.IsSuccessful.Should().BeTrue();
        }

        [Test]
        public async Task FindAllTagsOnWorkspaceAsync_ShouldReturnTagsList()
        {
            var response = await _client.FindAllTagsOnWorkspaceAsync(_workspaceId);
            response.IsSuccessful.Should().BeTrue();
        }

        [Test]
        public async Task CreateTagAsync_ShouldCreteTagAndReturnTagDto()
        {
            var tagRequest = new TagRequest
            {
                Name = "Test tag",
            };
            var createResult = await _client.CreateTagAsync(_workspaceId, tagRequest);
            createResult.IsSuccessful.Should().BeTrue();
            createResult.Data.Should().NotBeNull();

            var findResult = await _client.FindAllTagsOnWorkspaceAsync(_workspaceId);
            findResult.IsSuccessful.Should().BeTrue();
            findResult.Data.Should().ContainEquivalentOf(createResult.Data);
        }

        [Test]
        public async Task CreateTagAsync_NullName_ShouldThrowArgumentException()
        {
            var tagRequest = new TagRequest
            {
                Name = null,
            };
            Func<Task> create = () => _client.CreateTagAsync(_workspaceId, tagRequest);
            await create.Should().ThrowAsync<ArgumentException>()
                .WithMessage($"Value cannot be null. (Parameter '{nameof(TagRequest.Name)}')");
        }
    }
}