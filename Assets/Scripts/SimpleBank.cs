using System;
using System.Numerics;
using System.Threading.Tasks;
using ChainSafe.Gaming.Evm.Transactions;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using UnityEngine;
using ChainSafe.Gaming.RPC.Events;



namespace ChainSafe.Gaming.Evm.Contracts.Custom
{
    public partial class SimpleBank : ICustomContract
    {
        public string Address => OriginalContract.Address;
       
        public string ABI => "[ 	{ 		\"inputs\": [], 		\"name\": \"deposit\", 		\"outputs\": [], 		\"stateMutability\": \"payable\", 		\"type\": \"function\" 	}, 	{ 		\"inputs\": [], 		\"stateMutability\": \"nonpayable\", 		\"type\": \"constructor\" 	}, 	{ 		\"inputs\": [ 			{ 				\"internalType\": \"uint256\", 				\"name\": \"amount\", 				\"type\": \"uint256\" 			}, 			{ 				\"internalType\": \"bytes\", 				\"name\": \"signature\", 				\"type\": \"bytes\" 			} 		], 		\"name\": \"withdraw\", 		\"outputs\": [], 		\"stateMutability\": \"nonpayable\", 		\"type\": \"function\" 	}, 	{ 		\"inputs\": [], 		\"name\": \"backendAddress\", 		\"outputs\": [ 			{ 				\"internalType\": \"address\", 				\"name\": \"\", 				\"type\": \"address\" 			} 		], 		\"stateMutability\": \"view\", 		\"type\": \"function\" 	}, 	{ 		\"inputs\": [ 			{ 				\"internalType\": \"address\", 				\"name\": \"\", 				\"type\": \"address\" 			} 		], 		\"name\": \"balances\", 		\"outputs\": [ 			{ 				\"internalType\": \"uint256\", 				\"name\": \"\", 				\"type\": \"uint256\" 			} 		], 		\"stateMutability\": \"view\", 		\"type\": \"function\" 	} ]";
        
        public string ContractAddress { get; set; }
        
        public IEventManager EventManager { get; set; }

        public Contract OriginalContract { get; set; }
                
        public bool Subscribed { get; set; }

        
        #region Methods

        public async Task Deposit(TransactionRequest transactionOverwrite=null) 
        {
            var response = await OriginalContract.Send("deposit", new object [] {
            }, transactionOverwrite);
            
        }
        public async Task<TransactionReceipt> DepositWithReceipt( TransactionRequest transactionOverwrite=null) 
        {
            var response = await OriginalContract.SendWithReceipt("deposit", new object [] {
                
            }, transactionOverwrite);
            
            return response.receipt;
        }

        public async Task Withdraw(BigInteger amount, byte[] signature, TransactionRequest transactionOverwrite=null) 
        {
            var response = await OriginalContract.Send("withdraw", new object [] {
                amount, signature
            }, transactionOverwrite);
            
            
        }
        public async Task<TransactionReceipt> WithdrawWithReceipt(BigInteger amount, byte[] signature, TransactionRequest transactionOverwrite=null) 
        {
            var response = await OriginalContract.SendWithReceipt("withdraw", new object [] {
                amount, signature
            }, transactionOverwrite);
            
            return response.receipt;
        }

        public async Task<string> BackendAddress( TransactionRequest transactionOverwrite=null) 
        {
            var response = await OriginalContract.Call<string>("backendAddress", new object [] {
                
            }, transactionOverwrite);
            
            return response;
        }


        public async Task<BigInteger> Balances(string address, TransactionRequest transactionOverwrite=null) 
        {
            var response = await OriginalContract.Call<BigInteger>("balances", new object [] {
                address
            }, transactionOverwrite);
            
            return response;
        }



        #endregion
        
        
        #region Event Classes


        #endregion
        
        #region Interface Implemented Methods
        
        public async ValueTask DisposeAsync()
        {
            
            if(!Subscribed)
                return;
                
           
            Subscribed = false;
            try
            {
                if(EventManager == null)
                    return;

            
            
            }catch(Exception e)
            {
                Debug.LogError("Caught an exception whilst unsubscribing from events\n" + e.Message);
            }
        }
        
        public async ValueTask InitAsync()
        {
            if(Subscribed)
                return;
            Subscribed = true;

            try
            {
                if(EventManager == null)
                    return;

    
            }catch(Exception e)
            {
                Debug.LogError("Caught an exception whilst subscribing to events. Subscribing to events will not work in this session\n" + e.Message);
            }
            
        }
        
        [Obsolete("It's not advisable to use this method. Use the pre-generated methods instead.")]
        public IContract Attach(string address)
        {
            return OriginalContract.Attach(address);
        }
        
        [Obsolete("It's not advisable to use this method. Use the pre-generated methods instead.")]
        public Task<object[]> Call(string method, object[] parameters = null, TransactionRequest overwrite = null)
        {
            return OriginalContract.Call(method, parameters, overwrite);
        }
        
        [Obsolete("It's not advisable to use this method. Use the pre-generated methods instead.")]
        public object[] Decode(string method, string output)
        {
            return OriginalContract.Decode(method, output);
        }
        
        [Obsolete("It's not advisable to use this method. Use the pre-generated methods instead.")]
        public Task<object[]> Send(string method, object[] parameters = null, TransactionRequest overwrite = null)
        {
            return OriginalContract.Send(method, parameters, overwrite);
        }

        [Obsolete("It's not advisable to use this method. Use the pre-generated methods instead.")]
        public Task<(object[] response, TransactionReceipt receipt)> SendWithReceipt(string method, object[] parameters = null, TransactionRequest overwrite = null)
        {
            return OriginalContract.SendWithReceipt(method, parameters, overwrite);
        }
        
        [Obsolete("It's not advisable to use this method. Use the pre-generated methods instead.")]
        public Task<HexBigInteger> EstimateGas(string method, object[] parameters, TransactionRequest overwrite = null)
        {
            return OriginalContract.EstimateGas(method, parameters, overwrite);
        }
        
        [Obsolete("It's not advisable to use this method. Use the pre-generated methods instead.")]
        public string Calldata(string method, object[] parameters = null)
        {
            return OriginalContract.Calldata(method, parameters);
        }
        
        [Obsolete("It's not advisable to use this method. Use the pre-generated methods instead.")]
        public Task<TransactionRequest> PrepareTransactionRequest(string method, object[] parameters, bool isReadCall = false, TransactionRequest overwrite = null)
        {
            return OriginalContract.PrepareTransactionRequest(method, parameters, isReadCall, overwrite);
        }
        #endregion
    }


}
