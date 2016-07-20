using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;

namespace CorBaike.BaikeService
{
    public sealed class BaikeQueryService : IBackgroundTask
    {

        VoiceCommandServiceConnection voiceServiceConnection;

        BackgroundTaskDeferral serviceDeferral;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {

            serviceDeferral = taskInstance.GetDeferral();

            taskInstance.Canceled += OnTaskCanceled;

            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            if (triggerDetails != null && triggerDetails.Name == "BaikeQueryService")
            {
                try
                {
                    voiceServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);

                    voiceServiceConnection.VoiceCommandCompleted += OnVoiceCommandCompleted;
                    VoiceCommand voiceCommand = await voiceServiceConnection.GetVoiceCommandAsync();

                    switch (voiceCommand.CommandName)
                    {
                        case "showBaikeForKeyword":
                            string keyword = voiceCommand.Properties["keyword"][0];
                            await QueryBaikeByKeyword(keyword);
                            break;
                        default:
                            LaunchAppInForeground();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Handling Voice Command failed " + ex.ToString());
                }
            }
        }

        private async Task QueryBaikeByKeyword(string keyword)
        {
            var userProgressMessage = new VoiceCommandUserMessage();
            userProgressMessage.DisplayMessage = userProgressMessage.SpokenMessage = $"正在查询{keyword}";
            VoiceCommandResponse response = VoiceCommandResponse.CreateResponse(userProgressMessage);
            await voiceServiceConnection.ReportProgressAsync(response);

            var userMessage = new VoiceCommandUserMessage();

            var data = await QueryBaike.BaiduBaike.QueryByKeyword(keyword);

            userMessage.DisplayMessage = userMessage.SpokenMessage = data.Summary;

            VoiceCommandResponse queryResponse = null;
            if (data.Image != null)
                queryResponse = VoiceCommandResponse.CreateResponse(userMessage, new List<VoiceCommandContentTile>() { new VoiceCommandContentTile() { Image = data.Image, ContentTileType = VoiceCommandContentTileType.TitleWith280x140Icon } });
            else
                queryResponse = VoiceCommandResponse.CreateResponse(userMessage);

            queryResponse.AppLaunchArgument = keyword;

            await voiceServiceConnection.ReportSuccessAsync(queryResponse);
        }

        private void OnVoiceCommandCompleted(VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args)
        {
            if (this.serviceDeferral != null)
            {
                this.serviceDeferral.Complete();
            }
        }

        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (this.serviceDeferral != null)
            {
                this.serviceDeferral.Complete();
            }
        }

        private async void LaunchAppInForeground()
        {
            var userMessage = new VoiceCommandUserMessage();

            userMessage.SpokenMessage = "正在打开";

            var response = VoiceCommandResponse.CreateResponse(userMessage);

            response.AppLaunchArgument = "";

            await voiceServiceConnection.RequestAppLaunchAsync(response);
        }

    }
}
