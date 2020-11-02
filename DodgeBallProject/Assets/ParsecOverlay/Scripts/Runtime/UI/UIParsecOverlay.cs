using ParsecGaming;
using ParsecUnity;
using ParsecUnity.API;
using Rewired;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ParsecOverlay
{
    public class UIParsecOverlay : MonoBehaviour
    {
        //Connection status
        [Header("Status")] public Image statusImage = null;
        public Color statusColorNotConnected = Color.red;
        public Color statusColorConnecting = Color.yellow;
        public Color statusColorConnected = Color.green;

        [Header("Camera")] public Camera streamedCamera;

        //Panels
        [Header("Panels")] public GameObject panelsRoot = null;
        private bool _arePanelsMaximized = false;

        //Panel Request Code
        [Header("Panel Request Code")] public GameObject panelRequestCode = null;
        public Text textOpenURL = null;

        //Panel Invitation
        [Header("Panel Invitations")] public GameObject panelInvitations = null;
        public InputField invitationURLField = null;

        //Game Infos
        [Header("Game Infos")] public string gameID = "";
        public string gameName = "GodfatherJAM 2020/2021";
        public string gameDescription = "Just play & chill! \\[^.^]/";
        public int maxPlayers = 2;

        //Rewired players to Assign
        [Header("Rewired Mapping")] public List<string> rewiredPlayers;

        private int _parsecPlayerIdCount = 1;

        //Internal data
        enum STATUS
        {
            NOT_INITIALIZED = 0,
            REQUEST_CODE,
            CONNECTED,
        }

        private STATUS _status = STATUS.NOT_INITIALIZED;

        private ParsecStreamFull _streamer;
        private AudioListener _audioListener;
        private SessionResultDataData _authData;
        private SessionData _sessionData;
        private string _invitationURL = "";

        private Dictionary<uint, string> _rewiredPlayerByGuestIds = new Dictionary<uint, string>();
        private Dictionary<string, uint> _guestIdByRewiredPlayer = new Dictionary<string, uint>();
        private Dictionary<uint, CustomController[]> _customControllersByGuestIds = new Dictionary<uint, CustomController[]>();

        private void Awake()
        {
            if (null == streamedCamera) {
                streamedCamera = Camera.main;
            }

            _audioListener = FindObjectOfType<AudioListener>();

            _streamer = streamedCamera.gameObject.AddComponent<ParsecStreamFull>();
            _streamer.GameID = gameID;

            _streamer.onUserAuthenticated = new userAuthenticatedEvent();
            _streamer.onUserAuthenticated.AddListener(AuthenticationPoll);
            _streamer.GuestConnected += Streamer_GuestConnected;
            _streamer.GuestDisconnected += Streamer_GuestDisconnected;

            panelsRoot.SetActive(false);
            panelRequestCode.SetActive(false);
            panelInvitations.SetActive(false);
            statusImage.color = statusColorNotConnected;
        }

        public void ToggleMaximize()
        {
            _arePanelsMaximized = !_arePanelsMaximized;
            panelsRoot.SetActive(_arePanelsMaximized);

            if (_arePanelsMaximized && _status == STATUS.NOT_INITIALIZED) {
                RequestAccessCode();
            }
        }

        public void AuthenticationPoll(ParsecUnity.API.SessionResultDataData data,
            ParsecUnity.API.SessionResultEnum status)
        {
            switch (status) {
                case ParsecUnity.API.SessionResultEnum.PolledTooSoon:
                    break;
                case ParsecUnity.API.SessionResultEnum.Pending:
                    Debug.Log("Parsec: Waiting for response...");
                    break;
                case ParsecUnity.API.SessionResultEnum.CodeApproved:
                    Debug.Log("Parsec: Code Approved");
                    _authData = data;
                    StartParsec();
                    break;
                case ParsecUnity.API.SessionResultEnum.CodeInvallidExpiredDenied:
                    Debug.LogError("Parsec Error: Code Expired");
                    break;
                case ParsecUnity.API.SessionResultEnum.Unknown:
                    Debug.LogError("Parsec Error: Unknown State");
                    break;
                default:
                    break;
            }
        }

        public void RequestAccessCode()
        {
            _sessionData = _streamer.RequestCodeAndPoll();
            if ((_sessionData != null) && (_sessionData.data != null)) {
                textOpenURL.text = _sessionData.data.verification_uri + "/" + _sessionData.data.user_code;
                panelRequestCode.SetActive(true);
                panelInvitations.SetActive(false);
                _status = STATUS.REQUEST_CODE;
                statusImage.color = statusColorConnecting;
            }
        }

        public void OpenAccessCodeURL()
        {
            if ((_sessionData != null) && (_sessionData.data != null)) {
                string requestCodeURL = "https://parsecgaming.com/activate/" + _sessionData.data.user_code;
                Application.OpenURL(requestCodeURL);
            }
        }

        public void CopyAccessCodeURLToClipboard()
        {
            string requestCodeURL = "https://parsecgaming.com/activate/" + _sessionData.data.user_code;
            GUIUtility.systemCopyBuffer = requestCodeURL;
        }

        public void StartParsec()
        {
            _streamer.StartParsec(maxPlayers, false, gameName, gameDescription, _authData.id);
            invitationURLField.text = _invitationURL = _streamer.GetInviteUrl(_authData, 600, 10);
            panelInvitations.SetActive(true);
            panelRequestCode.SetActive(false);

            statusImage.color = statusColorConnected;
            _status = STATUS.CONNECTED;

            _audioListener.enabled = false;
            _audioListener.enabled = true;
        }

        public void StopParsec()
        {
            _streamer.StopParsec();
            _invitationURL = "";
            _status = STATUS.NOT_INITIALIZED;
            panelInvitations.SetActive(false);
            panelRequestCode.SetActive(true);
            panelsRoot.SetActive(false);
            _arePanelsMaximized = false;
            statusImage.color = statusColorNotConnected;
            _rewiredPlayerByGuestIds.Clear();
            _guestIdByRewiredPlayer.Clear();
        }

        public void CopyInvitationURLToClipboard()
        {
            GUIUtility.systemCopyBuffer = _invitationURL;
        }

        public void Streamer_GuestConnected(object sender, Parsec.ParsecGuest guest)
        {
            int parsecPlayerId = _parsecPlayerIdCount;
            _parsecPlayerIdCount++;
            if (rewiredPlayers.Count == 0) {
                Debug.LogWarning("ParsecManager : No rewired players to assign");
                return;
            }

            string rewiredPlayerId = _GetAvailableRewiredPlayer();
            CustomController csControllerJoystick = ReInput.controllers.CreateCustomController(0, "Parsec_" + guest.id);
            CustomController csControllerKeyboard = ReInput.controllers.CreateCustomController(1, "Parsec_" + guest.id);
            CustomController csControllerMouse = ReInput.controllers.CreateCustomController(2, "Parsec_" + guest.id);
            ParsecInput.AssignGuestToPlayer(guest, parsecPlayerId);
            Player rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);
            ParsecRewiredInput.AssignCustomControllerToUser(guest, csControllerJoystick);
            ParsecRewiredInput.AssignKeyboardControllerToUser(guest, csControllerKeyboard);
            ParsecRewiredInput.AssignMouseControllerToUser(guest, csControllerMouse);
            rewiredPlayer.controllers.AddController(csControllerJoystick, true);
            rewiredPlayer.controllers.AddController(csControllerKeyboard, true);
            rewiredPlayer.controllers.AddController(csControllerMouse, true);
            _rewiredPlayerByGuestIds.Add(guest.id, rewiredPlayerId);
            _guestIdByRewiredPlayer.Add(rewiredPlayerId, guest.id);
            _customControllersByGuestIds.Add(guest.id, new CustomController[] { csControllerJoystick, csControllerKeyboard, csControllerMouse });
        }

        public void Streamer_GuestDisconnected(object sender, Parsec.ParsecGuest guest)
        {
            Debug.Log("A guest has disconnected.");
            guest.state = Parsec.ParsecGuestState.GUEST_DISCONNECTED;

            ParsecRewiredInput.UnassignGuest(guest);
            ParsecInput.UnassignGuest(guest);

            if (_rewiredPlayerByGuestIds.ContainsKey(guest.id)) {
                string rewiredPlayerId = _rewiredPlayerByGuestIds[guest.id];
                if (_customControllersByGuestIds.ContainsKey(guest.id)) {
                    Player rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);
                    foreach (CustomController controller in _customControllersByGuestIds[guest.id]) {
                        rewiredPlayer.controllers.RemoveController<CustomController>(controller.id);
                        ReInput.controllers.DestroyCustomController(controller);
                    }
                }

                _rewiredPlayerByGuestIds.Remove(guest.id);
                _customControllersByGuestIds.Remove(guest.id);
                _guestIdByRewiredPlayer.Remove(rewiredPlayerId);
            }

            _parsecPlayerIdCount--;
        }


        private string _GetAvailableRewiredPlayer()
        {
            foreach (string rewiredPlayer in rewiredPlayers) {
                if (!_guestIdByRewiredPlayer.ContainsKey(rewiredPlayer)) {
                    return rewiredPlayer;
                }
            }

            return null;
        }

    }

}
