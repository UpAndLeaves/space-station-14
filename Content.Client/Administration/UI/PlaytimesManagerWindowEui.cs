using System.IO;
using Content.Client.Eui;
using Content.Client.Info.PlaytimeStats;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Content.Shared.Humanoid;
using Content.Shared.Preferences;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Player;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Utility;
using YamlDotNet.RepresentationModel;

namespace Content.Client.Administration.UI
{
    public sealed partial class PlaytimesManagerEui : BaseEui
    {
        [Dependency] private IFileDialogManager _dialogManager = default!;
        [Dependency] private ISerializationManager _serializationManager = default!;
        [Dependency] private ISawmill _sawmill = default!;


        private readonly PlaytimesManagerWindow _window;


        public PlaytimesManagerEui()
        {
            _window = new PlaytimesManagerWindow();
            _window.OnClose += () => SendMessage(new CloseEuiMessage());
            _window.ImportButton.OnPressed += ImportButtonOnOnPressed;
        }

        private async void ImportButtonOnOnPressed(BaseButton.ButtonEventArgs obj)
        {
            _window.ImportButton.Disabled = true;
            await using var file = await _dialogManager.OpenFile(new FileDialogFilters(new FileDialogFilters.Group("yml")), FileAccess.Read);

            if (file == null)
            {
                _window.ImportButton.Disabled = false;
                return;
            }

            try
            {
                using var reader = new StreamReader(file, EncodingHelpers.UTF8);
                var yamlStream = new YamlStream();
                yamlStream.Load(reader);

                var root = yamlStream.Documents[0].RootNode;
                PlaytimeInfoProfile profile = _serializationManager.Read<PlaytimeInfoProfile>(root.ToDataNode(), notNullableOverride: true);

                SendMessage(new PlaytimesManagerEuiMsg.PlaytimesData
                {
                    ForkID = profile.ForkID,
                    OverallPlaytime = profile.OverallPlaytime,
                    PlayerID = profile.PlayerID,
                    RolesPlaytimeList =  profile.RolesPlaytimeList,
                    UpdateType = (PlaytimeUpdateType?)_window.UpdateType.SelectedMetadata ?? PlaytimeUpdateType.Set
                });
            }
            catch (Exception exc)
            {
                _sawmill.Error($"Error when importing profile\n{exc.StackTrace}");
            }
            finally
            {
                _window.ImportButton.Disabled = false;
            }


        }

        public override void Opened()
        {
            _window.OpenCentered();
        }

        public override void Closed()
        {
            _window.Close();
        }
    }
}
