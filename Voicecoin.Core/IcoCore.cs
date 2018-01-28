using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.Text;
using Voicecoin.Core.Tables;
using System.Linq;
using Voicecoin.Core.Models;
using Coinbase.Models;

namespace Voicecoin.Core
{
    public class IcoCore
    {
        private Database dc;

        public IcoCore(Database db)
        {
            dc = db;
        }

        public IcoSummaryModel GetIcoStat()
        {
            var ico = (from currency in dc.Table<Cryptocurrency>()
                       join stage in dc.Table<PriceStage>() on new { currency.Symbol, currency.Status } equals new { stage.Symbol, stage.Status }
                       select new IcoSummaryModel
                       {
                           TotalSupply = currency.TotalSupply,
                           AvailableSupply = currency.AvailableSupply,
                           Symbol = currency.Symbol,
                           StartDate = stage.StartDate,
                           Base = stage.Base,
                           Amount = stage.Amount
                       }).First();

            return ico;
        }
    }
}
