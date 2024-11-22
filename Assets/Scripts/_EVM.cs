
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
         // Function to hash account address and amount (similar to web3.utils.soliditySha3)
        public  string SoliditySha3(string account, decimal amount)
        {
            // Remove "0x" prefix if present
            if (account.StartsWith("0x"))
                account = account.Substring(2);

            // Convert account address to byte array
            byte[] accountBytes = account.HexToByteArray();

            // Convert amount to Wei (1 Ether = 10^18 Wei) and then to a byte array
            BigInteger weiAmount = Nethereum.Web3.Web3.Convert.ToWei(amount);
            byte[] amountBytes = weiAmount.ToByteArray();

            // Ensure correct byte order (big-endian for Solidity compatibility)
            if (BitConverter.IsLittleEndian)
                Array.Reverse(amountBytes);

            // Concatenate the byte arrays
            byte[] combinedBytes = new byte[accountBytes.Length + amountBytes.Length];
            Buffer.BlockCopy(accountBytes, 0, combinedBytes, 0, accountBytes.Length);
            Buffer.BlockCopy(amountBytes, 0, combinedBytes, accountBytes.Length, amountBytes.Length);

            // Perform Keccak256 hash
            byte[] hashBytes = sha3.CalculateHash(combinedBytes);
            // Convert the byte array hash to a hex string
            string hashHex = "0x" + hashBytes.ToHex();
            return hashHex;
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

            Debug.Log($"balance BNB {eth}");

            object[] args =
            {
                3
            };

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
        private byte[] HexToByteArray(string hex)
        {
            int length = hex.Length / 2;
            byte[] bytes = new byte[length];

            for (int i = 0; i < length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }

            return bytes;
        }
        public static string ToHexString(string str)
        {
            var sb = new StringBuilder();

            var bytes = Encoding.Unicode.GetBytes(str);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString(); // returns: "48656C6C6F20776F726C64" for "Hello world"
        }
        public async void Withdraw()
        {
                // Step 1: Convert the Ether amount to Wei using BigInteger
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
             //   var gasEstimate = await simpleBank.EstimateGas("withdraw", new object[] { weiAmount, Encoding.UTF8.GetBytes(signedMessage)  });
                var resultx = await simpleBank.WithdrawWithReceipt(weiAmount, signedMessage2.HexToByteArray());
                Debug.Log("resultx "+ resultx.Status);
                return;

                // Remove the "0x" prefix, if it exists, to work with just the signature hex string
                // string signatureWithoutPrefix = signedMessage.StartsWith("0x") ? signedMessage.Substring(2) : signedMessage;
                // Debug.Log("signedMessage length: " + signatureWithoutPrefix.Length);
                // Debug.Log("signature : " + signatureWithoutPrefix);

                // // Step 3: Convert the signature to bytes (No Base64 decoding needed, it's hex)

                // // Step 4: Estimate gas for the withdraw function
                // var gasEstimate = await simpleBank.EstimateGas("withdraw", new object[] { weiAmount, signatureBytes });
                // Debug.Log("Gas Estimate: " + gasEstimate);



            // HexBigInteger _gasEstimate  = new HexBigInteger(2000 + gasEstimate.Value);
            // HexBigInteger _gasPrice  = new HexBigInteger(10000000000);
            // Debug.Log($"Withdrawing {eth} wei with signMessage {signMessage} and gas estimate {gasEstimate}");

          //  Debug.Log($"Withdraw! {result.Status} ");

        }

        public static string SignMessage(string privateKey, string message)
        {
            // Step 1: Hash the message
            var sha3 = new Sha3Keccack();
            byte[] encodedMessage = sha3.CalculateHash(Encoding.UTF8.GetBytes(message));

            // Step 2: Add Ethereum prefix (0x19Ethereum Signed Message:...)
            string prefix = "\x19Ethereum Signed Message:\n32";
            byte[] prefixedMessage = Encoding.UTF8.GetBytes(prefix);
            byte[] fullMessage = new byte[prefixedMessage.Length + encodedMessage.Length];
            Buffer.BlockCopy(prefixedMessage, 0, fullMessage, 0, prefixedMessage.Length);
            Buffer.BlockCopy(encodedMessage, 0, fullMessage, prefixedMessage.Length, encodedMessage.Length);

            // Step 3: Hash the prefixed message
            byte[] finalHash = sha3.CalculateHash(fullMessage);

            // Step 4: Sign the message with the private key
            var key = new EthECKey(privateKey);
            var signature = key.SignAndCalculateV(finalHash);

            // Step 5: Adjust recovery parameter (v) and pad r/s
            byte vByte = signature.V[0]; // Get the v byte (recovery parameter)
            string v = (vByte + 27).ToString("x2"); // Add 27 to adjust v

            // Pad r and s to 32 bytes
            string r = "0x" + BitConverter.ToString(signature.R).Replace("-", "").PadLeft(64, '0'); // Pad to 32 bytes
            string s = "0x" + BitConverter.ToString(signature.S).Replace("-", "").PadLeft(64, '0'); // Pad to 32 bytes

            // Return the final signature
            return r + s + v;
        }
        public static BigInteger ConvertEtherToWei(decimal amountInEther)
        {
            return Nethereum.Web3.Web3.Convert.ToWei(amountInEther);
        }
        public static string GenerateSoliditySha3(string account, string weiAmount)
        {
                 // Concatenate account and Wei amount
                var data = account + weiAmount;

                // Calculate Keccak-256 hash of the message
                var sha3 = new Sha3Keccack();

                
                var hash = sha3.CalculateHash(System.Text.Encoding.UTF8.GetBytes(data));
                
                return "0x" + hash.ToHex(); // Ensure the hash is prefixed with '0x'
        }
        // Hex to Bytes conversion (Helper function)
        public static byte[] HexToBytes(string hex)
        {
            if (hex.Length % 2 != 0) throw new ArgumentException("Invalid hex string length");
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }
        public void _GetBalances() =>GetBalances();

        public async void GetBalances()
        {
            Debug.Log($"Getting balances count {Web3Unity.Instance.Address}");
            var balance = await simpleBank.Balances(Web3Unity.Instance.Address);
            Debug.Log($"{Web3Unity.Instance.Address} balance is {balance}");
        }
    }


