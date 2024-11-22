using System.Collections;
using System.Numerics;
using ChainSafe.Gaming.Evm.JsonRpc;
using ChainSafe.Gaming.Evm.Providers;
using ChainSafe.Gaming.UnityPackage;
using ChainSafe.Gaming.Web3;
using ChainSafe.Gaming.Web3.Build;
using ChainSafe.Gaming.Web3.Unity;
using Nethereum.Hex.HexTypes;
using NUnit.Framework;
using Scripts.EVM.Token;
using UnityEngine;
using UnityEngine.TestTools;
using ChainSafe.Gaming.UnityPackage;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ChainSafe.Gaming.Evm.Contracts;
using ChainSafe.Gaming.Evm.Transactions;
using ChainSafe.Gaming.UnityPackage.Connection;
using ChainSafe.Gaming.UnityPackage.UI;
using ChainSafe.Gaming.Web3.Core.Evm;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
using ChainSafe.Gaming.RPC.Events;
public class Test1 : MonoBehaviour, ICustomContract
{
    public string ABI => throw new System.NotImplementedException();

    public string ContractAddress { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public Contract OriginalContract { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public IEventManager EventManager { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool Subscribed { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public string Address => throw new System.NotImplementedException();

    public IContract Attach(string address)
    {
        throw new System.NotImplementedException();
    }

    public Task<object[]> Call(string method, object[] parameters = null, TransactionRequest overwrite = null)
    {
        throw new System.NotImplementedException();
    }

    public string Calldata(string method, object[] parameters = null)
    {
        throw new System.NotImplementedException();
    }

    public object[] Decode(string method, string output)
    {
        throw new System.NotImplementedException();
    }

    public ValueTask DisposeAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task<HexBigInteger> EstimateGas(string method, object[] parameters, TransactionRequest overwrite = null)
    {
        throw new System.NotImplementedException();
    }

    public ValueTask InitAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task<TransactionRequest> PrepareTransactionRequest(string method, object[] parameters, bool isReadCall = false, TransactionRequest overwrite = null)
    {
        throw new System.NotImplementedException();
    }

    public Task<object[]> Send(string method, object[] parameters = null, TransactionRequest overwrite = null)
    {
        throw new System.NotImplementedException();
    }

    public Task<(object[] response, TransactionReceipt receipt)> SendWithReceipt(string method, object[] parameters = null, TransactionRequest overwrite = null)
    {
        throw new System.NotImplementedException();
    }
}
