﻿using FluentAssertions;
using HttpMocks.Implementation;
using HttpMocks.Thens;
using NUnit.Framework;

namespace HttpMocks.Tests.Unit.Implementation
{
    public class HandlingMockQueueTests : UnitTest
    {
        [Test]
        public void TestDequeueWhenEmpty()
        {
            var handlingMockQueue = new HandlingMockQueue(new HttpRequestMock[0]);
            handlingMockQueue.Dequeue("get", "/").Should().BeNull();
        }

        [Test]
        public void TestDequeueWhenOneMock()
        {
            var mocks = new[] {CreateMock("get")};
            var handlingMockQueue = new HandlingMockQueue(mocks);

            var handlingInfo = handlingMockQueue.Dequeue("get", "/");

            handlingInfo.Should().NotBeNull();
            handlingInfo.IsUsageCountValid().Should().BeTrue();
            handlingInfo.HasAttempts().Should().BeFalse();
            handlingInfo.UsageCount.ShouldBeEquivalentTo(1);

            handlingInfo = handlingMockQueue.Dequeue("get", "/");

            handlingInfo.Should().NotBeNull();
            handlingInfo.IsUsageCountValid().Should().BeFalse();
            handlingInfo.HasAttempts().Should().BeFalse();
            handlingInfo.UsageCount.ShouldBeEquivalentTo(2);
        }

        [Test]
        public void TestDequeueWhenChainMocks()
        {
            var mocks = new[] {CreateMock("get"), CreateMock("get")};
            var handlingMockQueue = new HandlingMockQueue(mocks);

            var handlingInfo = handlingMockQueue.Dequeue("get", "/");

            handlingInfo.Should().NotBeNull();
            handlingInfo.IsUsageCountValid().Should().BeTrue();
            handlingInfo.HasAttempts().Should().BeFalse();
            handlingInfo.UsageCount.ShouldBeEquivalentTo(1);

            handlingInfo = handlingMockQueue.Dequeue("get", "/");

            handlingInfo.Should().NotBeNull();
            handlingInfo.IsUsageCountValid().Should().BeTrue();
            handlingInfo.HasAttempts().Should().BeFalse();
            handlingInfo.UsageCount.ShouldBeEquivalentTo(1);
        }

        [Test]
        public void TestDequeueWhenChainMocksAndNotUsages()
        {
            var httpRequestMock = CreateMock("get");
            var mocks = new[] {CreateMock("get"), httpRequestMock};
            var handlingMockQueue = new HandlingMockQueue(mocks);

            var handlingInfo = handlingMockQueue.Dequeue("get", "/");

            handlingInfo.Should().NotBeNull();
            handlingInfo.IsUsageCountValid().Should().BeTrue();
            handlingInfo.HasAttempts().Should().BeFalse();
            handlingInfo.UsageCount.ShouldBeEquivalentTo(1);

            handlingInfo = handlingMockQueue.Dequeue("get", "/");

            handlingInfo.Should().NotBeNull();
            handlingInfo.IsUsageCountValid().Should().BeTrue();
            handlingInfo.HasAttempts().Should().BeFalse();
            handlingInfo.UsageCount.ShouldBeEquivalentTo(1);

            handlingInfo = handlingMockQueue.Dequeue("get", "/");

            handlingInfo.Should().NotBeNull();
            handlingInfo.IsUsageCountValid().Should().BeFalse();
            handlingInfo.HasAttempts().Should().BeFalse();
            handlingInfo.UsageCount.ShouldBeEquivalentTo(2);
            handlingInfo.ResponseMock.Should().Be(httpRequestMock.Response);
        }

        private static HttpRequestMock CreateMock(string method)
        {
            return new HttpRequestMock(method, "/")
            {
                Response = new HttpResponseMock {RepeatCount = 1}
            };
        }
    }
}