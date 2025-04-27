
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using ChainSafe.Gaming.UnityPackage;
using NUnit.Framework;
using Scripts.EVM.Token;
using ChainSafe.Gaming.Evm.Providers;
using Nethereum.Hex.HexTypes;
using TMPro;
using System;
using ChainSafe.Gaming.Evm.Contracts.Custom;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Threading.Tasks;
using ChainSafe.Gaming.Web3;
using System.Text;
using Nethereum.Util.ByteArrayConvertors;
using Nethereum.Web3;
using Nethereum.Util;
using WalletConnectSharp.Common.Utils;
using Nethereum.Signer;
using Nethereum.ABI;
using Nethereum.Model;
using System.Linq;

public class _EVM : MonoBehaviour
    {
        [SerializeField] private string contractAddress;
        [SerializeField] private string privateKey = "761e73222f5c2c9f540da203cefbdc1a83cb52f5c587ad7b0e13d0d470c335fc";

        public Text AmountUI;

        public void GetBalance() => StartCoroutine(TestNativeBalanceOf());
        CryptoRusherChain cryptoChain;
        public static _EVM Instance;

        public string testSignedMessage;
        void Awake()
        {
            Instance = this;
        }
       
        public async void InitializeContract () {
            var result = await Web3Unity.Instance.BuildContract<CryptoRusherChain>(contractAddress);
            cryptoChain = result ;
        }

        public IEnumerator TestNativeBalanceOf()
        {

            var getNativeBalanceOf = Web3Unity.Web3.RpcProvider.GetBalance(Web3Unity.Instance.Address);
            

            yield return new WaitUntil(() => getNativeBalanceOf.IsCompleted);
            HexBigInteger balance =   getNativeBalanceOf.Result;
            float wei = float.Parse(balance.ToString());
            float decimals = 1000000000000000000; // 18 decimals
            float eth = wei / decimals;
            UIManager.Instance.BNBUI.text = eth.ToString("0.0000000");

        }
        public void GetNativeBalanceOf() =>StartCoroutine(IGetNativeBalanceOf());
        public IEnumerator IGetNativeBalanceOf()
        {
            GameObject loading = UIManager.Instance.LoadingShow();
            loading.transform.SetAsLastSibling();
            var getNativeBalanceOf = Web3Unity.Web3.RpcProvider.GetBalance(Web3Unity.Instance.Address);
            
            yield return new WaitUntil(() => getNativeBalanceOf.IsCompleted);
            yield return new WaitForSeconds(1f);

            Destroy(loading);

            HexBigInteger balance =   getNativeBalanceOf.Result;
            float wei = float.Parse(balance.ToString());
            float decimals = 1000000000000000000; // 18 decimals
            float eth = wei / decimals;
            UIManager.Instance.BNBUI.text = eth.ToString("0.0000000");

        }
        [ContextMenu("_DepositAmount")]
        public void _DepositAmount() => DepositAmount(0.05f, () => {Debug.Log("Deposited");}) ;
        public async Task DepositAmount(float amountDeposit, Action action )
        {
            float eth = 0.002f;
            float decimals = 1000000000000000000; // 18 decimals

            float wei = eth * decimals;
            BigInteger amount = (BigInteger) wei;

            var result = await cryptoChain.DepositWithReceipt(new ChainSafe.Gaming.Evm.Transactions.TransactionRequest{
                Value = new HexBigInteger(amount),
            });
            action.Invoke();
            if (result.Status == new HexBigInteger(1))
            {
                UIManager.Instance.InstantiateMessagerPopPrefab_Message($"Bought booster success!!") ;
            }
          
        }

        public void _Withdraw() => Withdraw() ;

        public async void Withdraw(decimal _ethAmount = 0)
        {
            GameObject loading = UIManager.Instance.LoadingShow();
            loading.transform.SetAsLastSibling();

            decimal ethAmount = AmountUI.text.Count() > 0 ? decimal.Parse(AmountUI.text) : _ethAmount; // Using decimal for better precision
           // decimal ethAmount = decimal.Parse("0.003"); // Using decimal for better precision
            BigInteger weiAmount = Nethereum.Web3.Web3.Convert.ToWei(ethAmount, UnitConversion.EthUnit.Ether);

            var abiEncode = new ABIEncode();
            
            var encodedMessage =
            abiEncode.GetSha3ABIEncodedPacked(
                new ABIValue("address", Web3Unity.Instance.Address), new ABIValue("uint256",weiAmount));
                
            string message = "0x" + BitConverter.ToString(encodedMessage).Replace("-", "").ToLower();
            var signer = new EthereumMessageSigner();
            string signedMessage2 = signer.Sign(message.HexToByteArray(),new EthECKey(privateKey));
            var resultx = await cryptoChain.WithdrawWithReceipt(weiAmount, signedMessage2.HexToByteArray());
            
            if (resultx.Status == new HexBigInteger(1))
            {
                UIManager.Instance.InstantiateMessagerPopPrefab_Message($"Success withdrawn worth {ethAmount} of BNB") ;
                BoosterManager.Instance.BoosterShow();
                _EVM.Instance.GetNativeBalanceOf();
            }
            else
            {
                UIManager.Instance.InstantiateMessagerPopPrefab_Message($"Failed to withdraw worth {ethAmount} BNB") ;
            }
            Destroy(loading);
        }

        public void _GetBalances() =>GetBalances();

        public async void GetBalances()
        {
            // Debug.Log($"Getting balances count {Web3Unity.Instance.Address}");
            // var balance = await cryptoChain.Balances(Web3Unity.Instance.Address);
            // Debug.Log($"{Web3Unity.Instance.Address} balance is {balance}");
        }
    }


