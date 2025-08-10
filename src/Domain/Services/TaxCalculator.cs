using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Services;

public static class TaxCalculator
{
    public static decimal ComputeProgressiveTax(decimal income, IEnumerable<(decimal upTo, decimal rate)> brackets)
    {
        decimal tax = 0m;
        decimal previous = 0m;
        foreach (var bracket in brackets.OrderBy(b => b.upTo))
        {
            var amount = Math.Min(income, bracket.upTo) - previous;
            if (amount <= 0) break;
            tax += amount * bracket.rate;
            previous = bracket.upTo;
        }
        if (income > previous && brackets.Any())
        {
            var topRate = brackets.Last().rate;
            tax += (income - previous) * topRate;
        }
        return decimal.Round(tax, 2);
    }
}
