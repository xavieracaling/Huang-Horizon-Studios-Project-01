using System.Threading.Tasks;
using ChainSafe.Gaming.UnityPackage.Connection;
using ChainSafe.Gaming.Web3.Build;
using ChainSafe.Gaming.Web3.Core.Logout;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CWeb3 = ChainSafe.Gaming.Web3.Web3;
using ChainSafe.Gaming.UnityPackage;
    /// <summary>
    /// Controls used to easily connect/disconnect a wallet, display and copy address.
    /// </summary>
    public class _ConnectToWallet : ServiceAdapter, IWeb3InitializedHandler, ILogoutHandler
    {
        [SerializeField] private bool rememberMe = true;

        [Space] [SerializeField] private Button connectButton;

        [SerializeField] private Button disconnectButton;

        [Space] [SerializeField] private TextMeshProUGUI addressText;

        [SerializeField] private Button copyAddressButton;

        [Space]  public Transform ConnectedTransform;

        public Transform DisconnectedTransform;
        
        public static _ConnectToWallet Instance;
        void Awake()
        {
            Instance = this;
        }
        private async void Start()
        {
            DisconnectedTransform.gameObject.SetActive(true);
            try
            {
                await Web3Unity.Instance.Initialize(rememberMe);
            }
            finally
            {
                AddButtonListeners();

                ConnectionStateChanged(Web3Unity.Connected, Web3Unity.Instance.Address);
            }
        }

        private void AddButtonListeners()
        {
            connectButton.onClick.AddListener(Web3Unity.ConnectModal.Open);

            disconnectButton.onClick.AddListener(Disconnect);

            // copyAddressButton.onClick.AddListener(CopyAddress);

            // void CopyAddress()
            // {
            //     ClipboardManager.CopyText(addressText.text);
            // }
        }

        private void ConnectionStateChanged(bool connected, string address = "")
        {
            // ConnectedTransform.gameObject.SetActive(connected);
            
            // disconnectedTransform.gameObject.SetActive(!connected);
    
            if (connected)
            {
                UIManager.Instance.DisconnectedTransform.SetActive(false);
                _EVM.Instance.InitializeContract();
                PlayFabManager.Instance.CustomLogin(address);
                addressText.text = address;
            }
        }

        public Task OnWeb3Initialized(CWeb3 web3)
        {
            ConnectionStateChanged(true, web3.Signer.PublicAddress);

            return Task.CompletedTask;
        }

        private async void Disconnect()
        {
            await Web3Unity.Instance.Disconnect();
            UIManager.Instance.DisconnectedSceneUI();
        }

        public Task OnLogout()
        {
            UIManager.Instance.DisconnectedSceneUI();
            return Task.CompletedTask;
        }

        public override Web3Builder ConfigureServices(Web3Builder web3Builder)
        {
            return web3Builder.Configure(services =>
            {
                services.AddSingleton<ILogoutHandler, IWeb3InitializedHandler, _ConnectToWallet>(_ => this);
            });
        }
    }
