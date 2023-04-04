using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OviO.Integration;

public class OvioDeepLinker : MonoBehaviour
{
    private OviOIntegrationDeepLinkProcessing ovioIntegrationDeepLinkProcessing;

    void Awake()
    {
        ovioIntegrationDeepLinkProcessing = new OviOIntegrationDeepLinkProcessing(GameManager.ovioKey);
        ovioIntegrationDeepLinkProcessing.RegiserCallback(HandleTransactionData);
    }

	private void HandleTransactionData(TransactionData transactionData)
	{
		if (!transactionData.IsSuccess)
		{
			Debug.LogError($"OviOIntegration failed {transactionData.Message}");
		}
		else if (transactionData.CoinName != "Ruby")
		{
			//Handle not legal CoinName
			Debug.LogError("Not legal!");
		 }
		else
		{
			//Transfer transactionData.Amount to gamer

			Debug.Log("Success!");
		}

		/** Your own handling of transactionData
		 For example:
		 if (!transactionData.IsSuccess)
		 {
		     Debug.LogError($"OviOIntegration failed {transactionData.Message}");
		 }
		 else if (transactionData.CoinName != "<YOUR-GAME-COIN-NAME>")
		 {
		      Handle not legal CoinName
		 }
		 else
		 {
			  Transfer transactionData.Amount to gamer
		 }
		**/
	}
}
