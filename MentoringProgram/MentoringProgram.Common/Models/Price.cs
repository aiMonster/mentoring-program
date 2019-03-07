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

        //TODO: We should consider different currency 
        public static bool operator < (Price price1 , Price price2) =>        
             price1.Value < price2.Value;        

        //TODO: We should consider different currency 
        public static bool operator > (Price price1, Price price2) =>        
            price1.Value > price2.Value;
        
        public static explicit operator Price (decimal value) =>
            new Price(value);        

        //TODO: implement converter e.g. usd.toUah
    }
}
