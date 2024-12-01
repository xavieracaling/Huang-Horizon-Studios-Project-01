
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
        private static readonly Sha3Keccack sha3 = new Sha3Keccack();
        [SerializeField] private string contractAddress;
        [SerializeField] private string privateKey = "e992704ff76a7fc1529d0d7a4612a4818935b11f45c52b1ab01156155c808907";

        public Text AmountUI;

        public void GetBalance() => StartCoroutine(TestNativeBalanceOf());
        SimpleBank simpleBank;
        public static _EVM Instance;

        public string testSignedMessage;
        void Awake()
        {
            Instance = this;
        }
       
        public async void InitializeContract () {
            
            Debug.Log($"start build");
            var result = await Web3Unity.Instance.BuildContract<SimpleBank>(contractAddress);
            simpleBank = result ;
            Debug.Log($"{result.OriginalContract}, build");
        }

        public IEnumerator TestNativeBalanceOf()
        {

            var getNativeBalanceOf = Web3Unity.Web3.RpcProvider.GetBalance(Web3Unity.Instance.Address);
            

            yield return new WaitUntil(() => getNativeBalanceOf.IsCompleted);
            HexBigInteger balance =   getNativeBalanceOf.Result;
            float wei = float.Parse(balance.ToString());
            float decimals = 1000000000000000000; // 18 decimals
            float eth = wei / decimals;
            UIManager.Instance.BNBUI.text = eth.ToString("0.00000");
            Debug.Log($"balance BNB {eth}");

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
            UIManager.Instance.BNBUI.text = eth.ToString("0.00000");
            Debug.Log($"balance BNB {eth}");

        }
        public void _DepositAmount() => DepositAmount() ;
        public async void DepositAmount()
        {
            float eth = float.Parse(AmountUI.text.ToString());
            float decimals = 1000000000000000000; // 18 decimals

            float wei = eth * decimals;
            BigInteger amount = (BigInteger) wei;
            Debug.Log($"Depositing {amount} wei");

            var result = await simpleBank.DepositWithReceipt(new ChainSafe.Gaming.Evm.Transactions.TransactionRequest{
                Value = new HexBigInteger(amount),
            });
            Debug.Log($"Deposited! {result.Status} ");

        }
        public void _Withdraw() => Withdraw() ;

        public async void Withdraw()
        {
                decimal ethAmount = decimal.Parse(AmountUI.text); // Using decimal for better precision
                BigInteger weiAmount = Nethereum.Web3.Web3.Convert.ToWei(ethAmount, UnitConversion.EthUnit.Ether);
                // Step 2: Sign the message (address + amount)

                var abiEncode = new ABIEncode();
                
                var encodedMessage =
                abiEncode.GetSha3ABIEncodedPacked(
                    new ABIValue("address", Web3Unity.Instance.Address), new ABIValue("uint256",weiAmount));
                 

                Debug.Log($"Private Key: {privateKey}");
                Debug.Log($"Address: {Web3Unity.Instance.Address}");

              

                    
                string message = "0x" + BitConverter.ToString(encodedMessage).Replace("-", "").ToLower();
                var signer = new EthereumMessageSigner();
                string signedMessage2 = signer.Sign(message.HexToByteArray(),new EthECKey(privateKey));
                
                Debug.Log($"messagehash: {message}");
                Debug.Log($"Signed Message: {signedMessage2}");
                var resultx = await simpleBank.WithdrawWithReceipt(weiAmount, signedMessage2.HexToByteArray());
                Debug.Log("resultx "+ resultx.Status);
                return;


        }

        public void _GetBalances() =>GetBalances();

        public async void GetBalances()
        {
            Debug.Log($"Getting balances count {Web3Unity.Instance.Address}");
            var balance = await simpleBank.Balances(Web3Unity.Instance.Address);
            Debug.Log($"{Web3Unity.Instance.Address} balance is {balance}");
        }
    }


