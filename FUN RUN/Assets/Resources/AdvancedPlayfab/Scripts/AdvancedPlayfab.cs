using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using PlayFab;
using System.Threading.Tasks;
using PlayFab.ClientModels;
using Photon.Pun;
using Photon.VR;
using TMPro;

namespace AdvancedPlayFab
{
    public class AdvancedPlayfab : MonoBehaviour
    {

        public static AdvancedPlayfab instance;

        [Header("CURRENCY")]
        [Tooltip("The Currency Code for the currency you want in your game. (Default: HS)")]
        public string CurrencyCode = "HS";
        private int coins;
        [Header("VISUALIZERS")]
        [Tooltip("The text that will show the players PlayFab ID.")]
        public List<TextMeshPro> PlayFabIDText;
        [Tooltip("The text that will show the players currency.")]
        public List<TextMeshPro> CurrencyText;
        [Header("CATALOG ITEMS")]
        [Tooltip("The name of your catalog for mod cosmetics, regular cosmetics, etc. (Default: Catalog)")]
        public List<string> Catalogs = new List<string>() { "Catalog" };
        [Tooltip("Items to be enabled from your Catalogs.")]
        public List<GameObject> EnableItems;
        [Tooltip("Items to be disabled from your Catalogs.")]
        public List<GameObject> DisableItems;
        [Header("BAN ITEMS")]
        [Tooltip("Items to be enabled when your banned.")]
        public List<GameObject> BannedEnableItems;
        [Tooltip("Items to be disabled when your banned.")]
        public List<GameObject> BannedDisableItems;
        [Tooltip("If this is enabled, then Playfab will show your ban reason and time remaining. (Default: True)")]
        public bool BanStatusEnabled = true;
        [Tooltip("The text that shows your ban reason.")]
        public TextMeshPro BanReason;
        [Tooltip("The text that shows your ban reason.")]
        public TextMeshPro BanTime;
        [Header("NAME SAVING")]
        [Tooltip("If this is enabled, then Playfab Name Saving will happen from this script. (Default: True)")]
        public bool SavingEnabled = true;
        [Tooltip("The script where inputing your name takes place.")]
        public NameScript NamingPC;
        string oldusername;

        [HideInInspector]
        public string PlayFabID;
        [HideInInspector]
        public int CurrencyAmount;

        private bool isChecking;
        [HideInInspector]
        public bool loggingIn = false;

        [HideInInspector]
        public string titleID;

        public void Awake()
        {
            instance = this;
            loggingIn = false;
        }

        private void Start()
        {
            PlayfabStartup();
            InvokeRepeating("CheckPlayerStatus", 0f, 30f); // Credits to Cupid#9773
        }

        public void PlayfabStartup()
        {
            loggingIn = true;

            var request = new LoginWithCustomIDRequest
            {
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetPlayerProfile = true
                }
            };
            PlayFabClientAPI.LoginWithCustomID(request, LoginSuccess, OnError);
        }

        public void LoginSuccess(LoginResult result)
        {
            Debug.Log("Login Successful.");
            PhotonVRManager.Connect();
            GetAccountInfoRequest InfoRequest = new GetAccountInfoRequest();
            PlayFabClientAPI.GetAccountInfo(InfoRequest, AccountInfoSuccess, OnError);

            GetInventory();
            PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest { PlayFabId = PlayFabID }, OnGetPlayerProfileSuccess, OnError);
        }




        private void OnGetPlayerProfileSuccess(GetPlayerProfileResult result) // Credits to Cupid#9773
        {
            isChecking = false;

            if (result.PlayerProfile.BannedUntil.HasValue && result.PlayerProfile.BannedUntil.Value > DateTime.UtcNow)
            {
                Application.Quit();
            }
        }

        void OnGetPlayerProfileError(PlayFabError error) // Credits to Cupid#9773
        {
            isChecking = false;

            if (error.Error == PlayFabErrorCode.AccountBanned)
            {
                Application.Quit();
            }
        }

        void Update()
        {
            if (PlayFabClientAPI.IsClientLoggedIn() && SavingEnabled)
            {
                if (NamingPC.NameVar != oldusername)
                {
                    oldusername = NamingPC.NameVar;

                    PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
                    {
                        DisplayName = NamingPC.NameVar
                    }, delegate (UpdateUserTitleDisplayNameResult result)
                    {
                    }, delegate (PlayFabError error)
                    {
                        if (error.Error == PlayFabErrorCode.AccountBanned)
                        {
                            oldusername = "false";
                        }
                        if (error.Error == PlayFabErrorCode.AccountNotFound)
                        {
                            oldusername = "false";
                        }
                        if (error.Error == PlayFabErrorCode.AccountDeleted)
                        {
                            oldusername = "false";
                        }
                        if (error.Error == PlayFabErrorCode.APIClientRequestRateLimitExceeded)
                        {
                            oldusername = "false";
                        }
                        if (error.Error == PlayFabErrorCode.NotAuthenticated)
                        {
                            oldusername = "false";
                        }
                    }); ;
                }
            }

            if (!gameObject.activeSelf)
            {
                Application.Quit();
            }
        }

        public void AccountInfoSuccess(GetAccountInfoResult result)
        {
            PlayFabID = result.AccountInfo.PlayFabId;

            for (int i = 0; i < PlayFabIDText.Count; i++)
            {
                PlayFabIDText[i].text = PlayFabID;
            }

            PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            (result) =>
            {
                foreach (var item in result.Inventory)
                {
                    for (int i = 0; i < Catalogs.Count; i++)
                    {
                        if (Catalogs[i].Contains(item.CatalogVersion))
                        {
                            for (int a = 0; a < EnableItems.Count; a++)
                            {
                                if (EnableItems[a].name == item.ItemId)
                                {
                                    EnableItems[a].SetActive(true);
                                }
                            }
                            for (int a = 0; a < DisableItems.Count; a++)
                            {
                                if (DisableItems[a].name == item.ItemId)
                                {
                                    DisableItems[a].SetActive(false);
                                }
                            }
                        }
                    }
                }
            },
            (error) =>
            {
                Debug.LogError(error.GenerateErrorReport());
            });
        }

        public void GetInventory()
        {
            PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), UserInventorySuccess, OnError);
        }

        void UserInventorySuccess(GetUserInventoryResult result)
        {
            coins = result.VirtualCurrency[CurrencyCode];
            CurrencyAmount = coins;

            for (int i = 0; i < CurrencyText.Count; i++)
            {
                CurrencyText[i].text = CurrencyAmount.ToString();
            }
        }

        private void OnError(PlayFabError error)
        {
            if (error.Error == PlayFabErrorCode.AccountBanned)
            {
                PhotonNetwork.Disconnect();

                for (int i = 0; i < BannedEnableItems.Count; i++)
                {
                    BannedEnableItems[i].SetActive(true);
                }

                for (int i = 0; i < BannedDisableItems.Count; i++)
                {
                    BannedDisableItems[i].SetActive(false);
                }

                foreach (var item in error.ErrorDetails)
                {
                    if (BanStatusEnabled)
                    {
                        BanReason.text = item.Key;
                        string Expires = item.Value[0];
                        System.DateTime UnbanDate = System.DateTime.Parse(Expires, null, System.Globalization.DateTimeStyles.RoundtripKind);
                        System.DateTime CurrentDate = System.DateTime.UtcNow;
                        TimeSpan TimeRemaining = UnbanDate - CurrentDate;
                        double hoursRemaining = Math.Abs(TimeRemaining.TotalHours);
                        int hoursRemainingInt = (int)Math.Floor(hoursRemaining);
                        BanTime.text = hoursRemainingInt.ToString();
                    }
                }
            }
            else
            {
                PlayfabStartup();
            }
        }

        void CheckPlayerStatus() // Credits to Cupid#9773
        {
            if (!isChecking && PlayFabClientAPI.IsClientLoggedIn())
            {
                isChecking = true;

                var request = new GetPlayerProfileRequest
                {
                    ProfileConstraints = new PlayerProfileViewConstraints { ShowBannedUntil = true }
                };

                PlayFabClientAPI.GetPlayerProfile(request, OnGetPlayerProfileSuccess, OnGetPlayerProfileError);
            }
        }
    }
}