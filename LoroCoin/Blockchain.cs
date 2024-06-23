using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Transactions;

public class Blockchain
{
    //[JsonIgnore]
    public List<Block> Chain { get; private set; }

    public Blockchain()
    {
        InitializeBlockchain();
    }

    private void InitializeBlockchain()
    {
        Chain = new List<Block>();
        AddGenesisBlock();
    }

    private void AddGenesisBlock()
    {
        Chain.Add(new Block(DateTime.Now, null, new List<Transaction>()));
    }

    public void AddBlock(List<Transaction> transactions)
    {
        var previousBlock = GetLatestBlock();
        var newBlock = new Block(DateTime.Now, previousBlock.Hash, transactions);
        MineBlock(newBlock, 2); // Adjust the difficulty as needed
        Chain.Add(newBlock);
    }

    private void MineBlock(Block block, int difficulty)
    {
        //block.MineBlock(difficulty);
        block.MineBlock(3);//CHANGE DIFFICULTY HERE (num of 0's at the start of hash)
    }

    public bool IsValidChain()
    {
        for (int i = 1; i < Chain.Count; i++)
        {
            var currentBlock = Chain[i];
            var previousBlock = Chain[i - 1];

            // Validate the integrity of each block in the chain
            if (currentBlock.Hash != currentBlock.CalculateHash() || currentBlock.PreviousHash != previousBlock.Hash)
            {
                return false;
            }
        }

        return true;
    }

    public Block GetLatestBlock()
    {
        return Chain[^1]; // Equivalent to Chain[Chain.Count - 1] in C# 8.0 and later
    }

    public void AddBlock(List<Transaction> transactionsList, string senderPublicKey, string senderPrivateKey)
    {
        var previousBlock = GetLatestBlock();
        var newBlock = new Block(DateTime.Now, previousBlock.Hash, transactionsList);
        MineBlock(newBlock, 2); // Adjust the difficulty as needed

        // Sign the block with the sender's private key
        newBlock.SignBlock(senderPrivateKey);

        Chain.Add(newBlock);
    }
  
}
