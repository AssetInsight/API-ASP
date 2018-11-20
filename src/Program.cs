using System;
using System.Collections.Generic;
using AssetInsight.API;

namespace APIExample {
	class Program {
		static void Main(string[] args) {
			// Establish Connection to Helper Class
			var api = new establishConnection();

			// Get User Confirmation
			dynamic user = api.getUserConfirmation("example@example.com");
			Console.WriteLine("{0}", user);

			// Get Supported Assets
			dynamic supported = api.getSupported();
			Console.WriteLine("{0}", supported);

			// Get List of ALL REQUIRED Inspections for ALL Assets
			dynamic inspections = api.getInspections();
			Console.WriteLine("{0}", inspections);

			// Get Asset Requirements
			dynamic requirements = api.getAssetRequirements(566, 1);
			Console.WriteLine("{0}", requirements);

			// Create User Account If Doesn't Exist
			dynamic account = api.processNewAccount("example@example.com");
			Console.WriteLine("{0}", account);

			// Request an Analysis - Expects an object of the information
			object asset = new {
				type = 0,
				manufacturer = "cessna",
				model = 566,
				version = 1,
				manufacture = 1199923200,
				delivery = 1199923200,
				serial = "example",
				tail = "example",
				coverage = new {
					airframe = "msp",
					engines = "My Custom Program",
				},
                                modifications = new Dictionary<int, object> { 
                                        {3, new {id = 3, description = "Proline 21 single IFIS", installed = true}},
                                },
				inspections = new Dictionary<int, object> {
					{10, new {hours = 250, cycles = 175}},
					{11, new {hours = 3250, cycles = 2300}},
				},
			};

			/* Passing the user ID as a second parameter allows
			 * the analysis to be taged to another user with you
			 * as the referrer.  This will allow Asset Insight to
			 * better inform you about the activity you are generating
			 */
			dynamic results = api.processNewAnalysis(asset);
			Console.WriteLine("{0}", results);

			// Pause Console
			Console.ReadKey();
		}
	}
}
