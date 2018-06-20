/**
 * @file
 * Asset Insight Helper Class for Connecting to Asset Insight API.
 *
 * This file provides functions to simplify the API calls to Asset
 * Insight's RESTful API Service.  A valid key pair issued under the
 * express consent and authority of Asset Insight is required to use
 * Asset Insight RESTful API Services.
 * 
 * This class is for the sole benefit of Asset Insight subscribers and
 * community members.  It is not authorized for redistribution by third
 * parties without the express written consent of Asset Insight.
 */

using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AssetInsight.API {
	class establishConnection {
		HttpClient client = new HttpClient();

		protected string publicKey = "Your Public Key"; // TODO: Insert Your Public Key
		protected string privateKey = "Your Private Key"; // TODO: Insert Your Private Key
		protected string url = "";
		public string apiBase = "https://api.assetinsight.com/api/v1/";

		public dynamic results { get; set; }


		/**
		 * Execute RESTful API Get Request
		 * 
		 * @params 
		 *   endpoint - the endpoint location for the request
		 *   parameters - the query parameters for the request
		 *   format - the return format for the request
		 *   accessToken - the pregenerated Bearer Token
		 * @return
		 *   results: JSON Object Converted into a C# Object
		 */
		private async Task<IEnumerable<dynamic>> requestInformation(string method = "GET", string endpoint = "connection", dynamic parameters = null, string format = "json", string accessToken = null) {
			// Check if access token has expired
			if (accessToken == null) {
				accessToken = GetAccessToken();
			}

			// Establish Connection Object
			bool content = false;
			string url = null;
			dynamic connectionType = null;

			// Set Method Type
			switch (method.ToUpper()) {
				case "GET":
					connectionType = HttpMethod.Get;
					url = apiBase + endpoint + "?_format=" + format + "&" + parameters;
					break;
				case "POST":
					content = true;
					connectionType = HttpMethod.Post;
					url = apiBase + endpoint + "?_format=" + format;
					break;
			}


			var connection = new HttpRequestMessage(
					connectionType,
					string.Format(url)
			);

			// Add Headers
			connection.Headers.Add("Authorization", "Basic " + accessToken);
			connection.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			try {
				// Set Body if POST
				if (content) {
					// Convert Object to JSON then attach as string to Content of POST Request
					StringContent jsonQuery = new StringContent(JsonConvert.SerializeObject(parameters));
					connection.Content = jsonQuery;
					Console.WriteLine(JsonConvert.SerializeObject(parameters));
				}

				// Send Request
				HttpResponseMessage response = await client.SendAsync(connection);

				// Deserialize Response
				dynamic json = await response.Content.ReadAsStringAsync();
				results = JsonConvert.DeserializeObject<dynamic>(json);

				if (response.IsSuccessStatusCode) {
					return results;
				}
				else {
					Console.WriteLine("{0} ({1}): {2}", (int)response.StatusCode, response.ReasonPhrase, results.message);
					return null;
				}
			}
			catch (Exception e) {
				Console.WriteLine(e.Message);
			}

			return null;
		}

		/**
		 * Gets Authorization Code
		 * 
		 * @return - Encoded Key Pair to be sent for verification
		 */
		private string GetAccessToken() {
			// TODO: UPDATE TO REQUEST TOKEN FROM SERVER
			var basicAuth = Convert.ToBase64String(new UTF8Encoding().GetBytes(publicKey + ":" + privateKey));
			return basicAuth;
		}

		/**
		 * Get the required input fields for a given model and sub-version
		 * 
		 * @params 
		 *   asset_id - the id of the model to retrieve
		 *   version_id - the id of the sub-version to retrieve
		 * @return Designating available fields and if they are required
		 *   type: the asset type
		 *   manufacturer: machine name of the manufacturer of the model
		 *   model: the asset id of the model
		 *   version: the id of the sub-version
		 *   manufacture: timestamp of the date of delivery
		 *   serial: the asset serial number
		 *   tail: the asset secondary identification
		 *   modifications: array of modification ids
		 *   coverage: ARRAY
		 *     airframe: airframe coverage name (machine name preferred)
		 *     engines: engine coverage name (machine name preferred)
		 *     apu: apu coverage name (machine name preferred)
		 *     avionics: avionics coverage name (machine name preferred)
		 *   inspections: ARRAY of inspections each with the following format and keyed by Inspection ID
		 *     id: inspection id
		 *     description: human readable name
		 *     hours: last known completion or current time as integer
		 *     cycles: last known completion or current time as integer
		 *     date: last known completion or current time as timestamp        *     
		 */
		public IEnumerable<dynamic> getAssetRequirements(int asset_id, int version_id) {
			dynamic results = this.requestInformation("GET", "request/assetRequirements", "model=" + asset_id + "&version=" + version_id).Result;

			return results;
		}

		/**
		 * Gets currently supported asset groups available for analysis
		 * 
		 * @return - Keyed by id
		 *   asset_id: identifying id of the asset model
		 *   name: human readable name
		 *   manufacturer: machine name of the manufacturer of the model
		 *   manufacturer_type: the asset type
		 *   version_id: the model sub-version ID
		 *   version_name: human readable sub-version
		 */
		public IEnumerable<dynamic> getSupported() {
			dynamic results = this.requestInformation("GET", "request/supported", "type=model").Result;

			return results;
		}

		/**
		 * Checks to see if the user already has an account with Asset Insight
		 * 
		 * @params 
		 *   mail - the email to see if it has an account associated
		 * @return
		 *   id: Asset Insight user id
		 *   email: email of user
		 */
		public IEnumerable<dynamic> getUserConfirmation(string email) {
			dynamic results = this.requestInformation("GET", "request/userConfirmation", "email=" + email).Result;

			return results;
		}

		/**
		 * Checks to see if the user already has an account with Asset Insight
		 * 
		 * @params 
		 *   mail - the email to see if it has an account associated
		 * @return
		 *   id: Asset Insight user id
		 *   email: email of user
		 */
		public IEnumerable<dynamic> processNewAccount(string email) {
			object user = new {
				email = email
			};

			dynamic results = this.requestInformation("POST", "process/newAccount", user).Result;

			return results;
		}

		/**
		 * Checks to see if the user already has an account with Asset Insight
		 * 
		 * @params 
		 *   mail - the email to see if it has an account associated
		 * @return
		 *   id: Asset Insight user id
		 *   email: email of user
		 */
		public IEnumerable<dynamic> processNewAnalysis(object asset) {
			dynamic results = this.requestInformation("POST", "process/analysis", asset).Result;

			return results;
		}
	}
}