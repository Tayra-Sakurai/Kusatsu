// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Imazu.Models;
using Maiko.Contexts;
using Maiko.Models;
using Microsoft.Agents.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace Imazu.ViewModels
{
    public partial class AgentViewModel : ObservableObject
    {
        private readonly AIAgent agent;
        private readonly IDbContextFactory<MaikoContext> factory;
        private AgentSession? session;

        public AgentViewModel(IChatClient chatClient, IDbContextFactory<MaikoContext> factory, VectorStore vectorStore)
        {
            this.factory = factory;
            agent = new ChatClientAgent(
                chatClient,
                options: new()
                {
                    ChatOptions = new()
                    {
                        Instructions = "You are the user's budget management assistant.",
                        Tools = new List<AITool>
                        {
                            AIFunctionFactory.Create(GetLargeCategoriesAsync),
                            AIFunctionFactory.Create(GetMethodsAsync),
                        },
                    },
                    Name = "Perry",
                    AIContextProviders = new List<AIContextProvider>
                    {
                        new ChatHistoryMemoryProvider(
                        vectorStore,
                        "ChatHistory",
                        768,
                        session => new ChatHistoryMemoryProvider.State(
                            storageScope: new() { UserId = "user", SessionId = Guid.NewGuid().ToString() },
                            searchScope: new() { UserId = "user" })),
                    },
                });
            TextData = [];
            ToolApprovalRequestContents = [];
        }

        public ObservableCollection<ChatTextData> TextData { get; }
        public ObservableCollection<ToolApprovalRequestContent> ToolApprovalRequestContents { get; }

        [Description("Asynchronously gets all categories as a tree of categories and associated items.")]
        public async Task<List<LargeCategory>> GetLargeCategoriesAsync()
        {
            using MaikoContext context = await factory.CreateDbContextAsync();

            return await context.LargeCategories
                .Include(lc => lc.MediumCategories)
                .ThenInclude(mc => mc.SmallCategories)
                .ThenInclude(sc => sc.Items)
                .AsNoTracking()
                .ToListAsync();
        }

        [Description("Gets all payment methods with the related item and remainder data.")]
        public async Task<List<Method>> GetMethodsAsync()
        {
            using MaikoContext context = await factory.CreateDbContextAsync();

            return await context.Methods
                .Include(mtd => mtd.Items)
                .AsNoTracking()
                .ToListAsync();
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanInvoke))]
        private async Task InvokeAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt) ||
                session == null)
                return;

            TextData.Add(new() { ChatRole = ChatRole.User, Text = prompt });

            AgentResponse response = await agent.RunAsync(prompt, session);

            ProcessResponse(response);
        }

        private void ProcessResponse(AgentResponse response)
        {
            if (!string.IsNullOrEmpty(response.Text))
                TextData.Add(new ChatTextData { ChatRole = ChatRole.Assistant, Text = response.Text });

            IEnumerable<ToolApprovalRequestContent> approvalRequestContents = response.Messages
                .SelectMany(message => message.Contents)
                .OfType<ToolApprovalRequestContent>();

            foreach (
                ToolApprovalRequestContent toolApprovalRequestContent in
                approvalRequestContents)
                ToolApprovalRequestContents.Add(toolApprovalRequestContent);
        }

        private static bool CanInvoke(string prompt)
        {
            return !string.IsNullOrWhiteSpace(prompt);
        }

        public async Task StartSessionAsync()
        {
            session = await agent.CreateSessionAsync();
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanRespondToolRequest))]
        private async Task ApproveToolAsync(IEnumerable<object>? toolApprovalRequestContents)
        {
            if (toolApprovalRequestContents is null ||
                !toolApprovalRequestContents.OfType<ToolApprovalRequestContent>().Any())
                return;

            List<ToolApprovalRequestContent> toolApprovalRequestContents1 = [.. toolApprovalRequestContents.OfType<ToolApprovalRequestContent>()];

            ChatMessage message = new(
                ChatRole.User,
                [.. toolApprovalRequestContents1.Select(content => content.CreateResponse(true))]);
            AgentResponse response = await agent.RunAsync(message, session);

            foreach (
                ToolApprovalRequestContent toolApprovalRequestContent in toolApprovalRequestContents1)
                ToolApprovalRequestContents.Remove(toolApprovalRequestContent);

            ProcessResponse(response);
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanRespondToolRequest))]
        private async Task RejectToolAsync(IEnumerable<object>? toolApprovalRequestContents)
        {
            if (toolApprovalRequestContents is null ||
                !toolApprovalRequestContents.OfType<ToolApprovalRequestContent>().Any())
                return;

            IEnumerable<ToolApprovalRequestContent> toolApprovalRequestContents1 = toolApprovalRequestContents.OfType<ToolApprovalRequestContent>();
            ChatMessage message = new(
                ChatRole.User,
                [.. toolApprovalRequestContents1.Select(content => content.CreateResponse(false))]);
            AgentResponse response = await agent.RunAsync(message, session);

            foreach (
                ToolApprovalRequestContent toolApprovalRequestContent in toolApprovalRequestContents1)
                ToolApprovalRequestContents.Remove(toolApprovalRequestContent);

            ProcessResponse(response);
        }

        private static bool CanRespondToolRequest(IEnumerable<object>? toolApprovalRequestContents)
        {
            return toolApprovalRequestContents is not null &&
                toolApprovalRequestContents.OfType<ToolApprovalRequestContent>().Any();
        }
    }
}
