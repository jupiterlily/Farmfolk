using System.Collections.Generic;

// an economic entity is a person, group, or organization that has an interest in the economy. they can be a producer, consumer, or both
public class EconomicEntity
{
	     public string Name
	     {
		          get; private set;
	     }
	     public Dictionary<Product, ProductState> Products
	     {
		          get; private set;
	     }
	     public List<Trade> Trades
	     {
		          get; private set;
	     }
	     public List<Bid> Bids
	     {
		          get; private set;
	     }

	     public EconomicEntity(string name)
	     {
		          Name = name;
		          Products = new Dictionary<Product, ProductState>();
		          Trades = new List<Trade>();
		          Bids = new List<Bid>();
	     }

	     public void AddProduct(Product product, int amount)
	     {
		          MarketUtilities.UpdateInventory(Products, product, amount);
		          //debug log
		          UnityEngine.Debug.Log($"{Name} has added {amount} {product.Name} to their inventory.");
	     }

	     public void AddDesiredProduct(Product product, int amount)
	     {
		          MarketUtilities.UpdateDesiredInventory(Products, product, amount);
		          //debug log
		          UnityEngine.Debug.Log($"{Name} has added {amount} {product.Name} to their desired inventory.");
	     }

	     public void ShowProduct(Product product, float price)
	     {
		          MarketUtilities.UpdatePrice(Products, product, price);
		          //debug log
		          UnityEngine.Debug.Log($"{Name} has seen {product.Name} worth {price}.");
	     }

	     public void AddTrade(Trade trade)
	     {
		          Trades.Add(trade);

	     }

	     public void AddBid(Bid bid)
	     {
		          Bids.Add(bid);
	     }

	     public void RemoveTrade(Trade trade)
	     {
		          Trades.Remove(trade);
	     }

	     public void RemoveBid(Bid bid)
	     {
		          Bids.Remove(bid);
	     }

	     public void UpdateBid(Bid bid)
	     {
		          for (int i = 0; i < Bids.Count; i++)
		          {
				if (Bids[i].Bidder == bid.Bidder)
				{
					     Bids[i] = bid;
					     break;
				}
		          }
	     }
}


// a product is a good or service that can be bought or sold
public class Product
{
	     // name of the product
	     public string Name
	     {
		          get; private set;
	     }

	     public Product(string name)
	     {
		          Name = name;
	     }

	     public override bool Equals(object obj)
	     {
		          return obj is Product otherProduct && Name == otherProduct.Name;
	     }

	     public override int GetHashCode()
	     {
		          return Name.GetHashCode();
	     }
}
// a product state is the state of a product for an economic entity
public class ProductState
{
	     // how much of this item the economic unit has
	     public int Inventory
	     {
		          get; set;
	     }
	     // how much of this item the economic unit wants to have in inventory
	     public int DesiredInventory
	     {
		          get; set;
	     }
	     // amount of this item the economic unit wants to trade
	     public int SupplyDemand
	     {
		          get; set;
	     }
	     // how willing the economic unit is to trade this item. looks at supply and demand and price history and sets a value
	     public float ExchangeValue
	     {
		          get; set;
	     }
	     // how many times this item has been seen
	     public int TimesSeen
	     {
		          get; private set;
	     }
	     // the lowest price this economic unit has seen for this item
	     public float LowestPrice
	     {
		          get; private set;
	     }
	     // the average price this economic unit has seen for this item
	     public float AveragePrice
	     {
		          get; private set;
	     }
	     public float RecentPrice
	     {
		          get; private set;
	     }
	     // the highest price this economic unit has seen for this item
	     public float HighestPrice
	     {
		          get; private set;
	     }

	     public ProductState(int inventory, int amount)
	     {
		          Inventory = inventory;
		          DesiredInventory = inventory + amount;
		          SupplyDemand = amount;
		          ExchangeValue = amount * -1;
		          LowestPrice = 1;
		          AveragePrice = 1;
		          HighestPrice = 1;
	     }

	     // update the price history of this item
	     public void UpdatePrice(float price)
	     {
		          if (price < LowestPrice)
		          {
				LowestPrice = price;
		          }
		          if (price > HighestPrice)
		          {
				HighestPrice = price;
		          }
		          AveragePrice = ((AveragePrice * TimesSeen) + price) / 2;
		          // add the price to the recent price and divide by 2
		          RecentPrice = (RecentPrice + price) / 2;
		          //debug log
	     }

	     // update the desired inventory of this item
	     public void UpdateDesiredInventory(int amount)
	     {
		          DesiredInventory += amount;
		          SupplyDemand = DesiredInventory - Inventory;
	     }

	     // update exchange readiness of this item. looks at supply and demand and price history and sets a value
	     public void UpdateExchangeValue(int amount)
	     {
		          ExchangeValue += amount;
	     }

	     // update the inventory of this item
	     public void UpdateInventory(int amount)
	     {
		          Inventory += amount;
		          SupplyDemand = DesiredInventory - Inventory;
	     }

	     // update the supply and demand of this item
	     public void UpdateSupplyDemand(int amount)
	     {
		          SupplyDemand += amount;
	     }

	     // reset the times seen of this item
	     public void ResetTimesSeen()
	     {
		          TimesSeen = 0;
	     }

	     // update the times seen of this item
	     public void SeePrice(float price)
	     {
		          TimesSeen++;
		          UpdatePrice(price);
	     }
}
public class Market
{
	     public Dictionary<EconomicEntity, List<Trade>> Participants
	     {
		          get; private set;
	     }
	     public Dictionary<Product, ProductState> Products
	     {
		          get; private set;
	     }

	     public Market(List<EconomicEntity> participants = null, List<Product> products = null)
	     {
		          Participants = new Dictionary<EconomicEntity, List<Trade>>();
		          Products = new Dictionary<Product, ProductState>();

		          if (participants != null)
		          {
				foreach (var participant in participants)
				{
					     AddParticipant(participant);
				}
		          }

		          if (products != null)
		          {
				foreach (var product in products)
				{
					     if (!Products.ContainsKey(product))
					     {
						          Products.Add(product, new ProductState(0, 0));
					     }
				}
		          }
	     }

	     public void AddParticipant(EconomicEntity entity)
	     {
		          Participants.Add(entity, entity.Trades);
		          foreach (var product in entity.Products.Keys)
		          {
				MarketUtilities.UpdateInventory(Products, product, entity.Products[product].Inventory);
				MarketUtilities.UpdateDesiredInventory(Products, product, entity.Products[product].SupplyDemand);
				MarketUtilities.UpdatePrice(Products, product, entity.Products[product].ExchangeValue);
		          }
		          UnityEngine.Debug.Log($"{entity.Name} has been added to the market.");
	     }

	     public void UpdateMarketProducts()
	     {
		          foreach (var entity in Participants.Keys)
		          {
				foreach (var product in entity.Products.Keys)
				{
					     if (!Products.ContainsKey(product) && (entity.Products[product].Inventory > 0 || entity.Products[product].SupplyDemand != 0))
					     {
						          Products.Add(product, new ProductState(0, 0));
						          MarketUtilities.UpdatePrice(Products, product, entity.Products[product].ExchangeValue);
					     }
				}
		          }
		          UnityEngine.Debug.Log("Market products have been updated.");
	     }

	     public void UpdateEntityProducts()
	     {
		          foreach (var entity in Participants.Keys)
		          {
				foreach (var product in Products.Keys)
				{
					     if (!entity.Products.ContainsKey(product))
					     {
						          entity.Products.Add(product, new ProductState(0, 0));
						          MarketUtilities.UpdatePrice(entity.Products, product, Products[product].ExchangeValue);
					     }
				}
		          }
		          UnityEngine.Debug.Log("Entity products have been updated.");
	     }

	     public void RemoveParticipant(EconomicEntity entity)
	     {
		          Participants.Remove(entity);
		          foreach (var product in entity.Products.Keys)
		          {
				if (Products.ContainsKey(product))
				{
					     MarketUtilities.UpdateDesiredInventory(Products, product, entity.Products[product].SupplyDemand * -1);
					     MarketUtilities.UpdateInventory(Products, product, entity.Products[product].Inventory * -1);
				}
		          }
		          UnityEngine.Debug.Log($"{entity.Name} has been removed from the market.");
	     }

	     public void UpdateMarketInventory(Product product, int amount)
	     {
		          MarketUtilities.UpdateInventory(Products, product, amount);
		          UnityEngine.Debug.Log($"{product.Name} inventory has been updated.");
	     }

	     public void UpdateMarketDesiredInventory(Product product, int amount)
	     {
		          MarketUtilities.UpdateDesiredInventory(Products, product, amount);
		          UnityEngine.Debug.Log($"{product.Name} desired inventory has been updated.");
	     }
}


public class Trade
{
	     public EconomicEntity Seller
	     {
		          get; private set;
	     }
	     // What the seller is selling
	     public Bid SellerBid
	     {
		          get; private set;
	     }
	     // What the seller is willing to offer for sale
	     public List<Bid> BuyerBids
	     {
		          get; private set;
	     }
	     public Trade(EconomicEntity seller, Bid sellerBid)
	     {
		          Seller = seller;
		          SellerBid = sellerBid;
		          BuyerBids = new List<Bid>();
	     }
	     public void AddBuyerBid(Bid buyerBid)
	     {

		          BuyerBids.Add(buyerBid);
	     }
	     public void RemoveBuyerBid(Bid buyerBid)
	     {

		          BuyerBids.Remove(buyerBid);
	     }
	     public void AcceptBid(Bid buyerBid)
	     {

		          SellerBid.AcceptBid();
		          buyerBid.AcceptBid();
		          foreach (var bid in BuyerBids)
		          {
				if (bid != buyerBid)
				{
					     RemoveBuyerBid(bid);
				}
		          }
	     }
	     public void UpdateBuyerBid(EconomicEntity buyer, Bid newBid)
	     {
		          foreach (var bid in BuyerBids)
		          {
				if (bid.Bidder == buyer)
				{
					     RemoveBuyerBid(bid);
					     AddBuyerBid(newBid);
				}
		          }
	     }
	     //see which bid is worth the most to the seller
	     public Bid GetBestBid()
	     {

		          Bid bestBid = null;
		          float bestValue = 0;
		          //go through the bids 
		          foreach (var bid in BuyerBids)
		          {
				if (MarketUtilities.CalculateValueOfProducts(bid.Offer, Seller.Products) > bestValue)
				{
					     bestBid = bid;
				}
		          }
		          return bestBid;
	     }
}

public class Bid
{
	     public EconomicEntity Bidder
	     {
		          get; private set;
	     }
	     public Dictionary<Product, int> Offer
	     {
		          get; private set;
	     }

	     public float ValueToBidder
	     {

		          get; private set;
	     }
	     public bool IsAccepted
	     {
		          get; private set;
	     }

	     public Bid(EconomicEntity bidder, Dictionary<Product, int> products)
	     {
		          Bidder = bidder;
		          Offer = products;
		          ValueToBidder = MarketUtilities.CalculateValueOfProducts(products, bidder.Products);
		          IsAccepted = false;
	     }

	     // Add a product to the bid
	     public void AddProductToBid(Product product, int amount)
	     {
		          // If the product is already in the bid, update the amount
		          if (Offer.ContainsKey(product))
		          {
				Offer[product] += amount;
		          }
		          else
		          {
				Offer.Add(product, amount);
		          }
		          CalculateValueOfBid(Bidder.Products);
	     }

	     // Remove a product from the bid
	     public void RemoveProductFromBid(Product product, int amount)
	     {
		          if (Offer.ContainsKey(product))
		          {
				Offer.Add(product, amount * -1);
		          }
		          else
		          {
		          }
		          CalculateValueOfBid(Bidder.Products);
	     }

	     // Accept the bid
	     public void AcceptBid()
	     {
		          IsAccepted = true;
	     }

	     public float CalculateValueOfBid(Dictionary<Product, ProductState> valuer)
	     {
		          return MarketUtilities.CalculateValueOfProducts(Offer, valuer);
	     }
}


