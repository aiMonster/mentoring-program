using MentoringProgram.Common.Enums;
using System;

namespace MentoringProgram.Common.Models
{
    public struct Price
    {
        public decimal Value { get; }
        public Currency Currency { get; }

        public Price(decimal priceValue, Currency currency = Currency.USD)
        {
            if(priceValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(priceValue));
            }

            Value = priceValue;
            Currency = currency;
        }
       
        public static bool operator < (Price price1 , Price price2)
        {
            if(price1.Currency != price2.Currency)
            {
                throw new NotImplementedException();
            }

            return price1.Value < price2.Value;
        }
        
        public static bool operator >(Price price1, Price price2)
        {
            if (price1.Currency != price2.Currency)
            {
                throw new NotImplementedException();
            }

            return price1.Value > price2.Value;
        }
           
        
        public static explicit operator Price (decimal value) =>
            new Price(value);

        public override string ToString()
        {    
            return Value + " " + Currency.ToString();
        }

        //TODO: implement converter e.g. usd.toUah
    }
}
