using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GK2823.ModelLib.Finance.API
{
    public class PoolDetail
    {
        public int id { get; set; }
        public decimal break_limit_down_times { get; set; }
        public decimal break_limit_up_times { get; set; }
        public decimal buy_lock_volume_ratio { get; set; }
        public decimal change_percent { get; set; }
        public decimal first_break_limit_down { get; set; }
        public decimal first_break_limit_up { get; set; }
        public decimal first_limit_down { get; set; }
        public decimal first_limit_up { get; set; }
        public bool is_new_stock { get; set; }
        public decimal issue_price { get; set; }
        public decimal last_break_limit_down { get; set; }
        public decimal last_break_limit_up { get; set; }
        public decimal last_limit_down { get; set; }
        public decimal last_limit_up { get; set; }
        public decimal limit_down_days { get; set; }
        public LimitTimeline limit_timeline { get; set; }

        public class LimitTimeline
        {
            public List<Items> items { get; set; }
            public class Items
            {
                public decimal timestamp { get; set; }
                public decimal status { get; set; }
            }
        }
        public decimal limit_up_days { get; set; }
        public decimal listed_date { get; set; }
        public decimal m_days_n_boards_boards { get; set; }
        public decimal m_days_n_boards_days { get; set; }
        public decimal mtm { get; set; }
        public decimal nearly_new_acc_pcp { get; set; }
        public decimal nearly_new_break_days { get; set; }
        public decimal new_stock_acc_pcp { get; set; }
        public decimal new_stock_break_limit_up { get; set; }
        public decimal new_stock_limit_up_days { get; set; }
        public decimal new_stock_limit_up_price_before_broken { get; set; }
        public decimal non_restricted_capital { get; set; }
        public decimal price { get; set; }
        public decimal sell_lock_volume_ratio { get; set; }
        public string stock_chi_name { get; set; }
        public decimal stock_type { get; set; }
        public SurgeReason surge_reason { get; set; }
        public class SurgeReason
        {
            public string symbol { get; set; }
            public string stock_reason { get; set; }
            public List<RelatedPlates> related_plates { get; set; }

            public class RelatedPlates
            {
                public decimal plate_id { get; set; }
                public string plate_name { get; set; }
                public string plate_reason { get; set; }
            }
        }
        public string symbol { get; set; }
        public decimal total_capital { get; set; }
        public decimal turnover_ratio { get; set; }
        public decimal volume_bias_ratio { get; set; }
        public decimal yesterday_break_limit_up_times { get; set; }
        public decimal yesterday_first_limit_up { get; set; }
        public decimal yesterday_last_limit_up { get; set; }
        public decimal yesterday_limit_down_days { get; set; }
        public decimal yesterday_limit_up_days { get; set; }

    }

    [Table("pool_detail")]
    public class _PoolDetail
    {
        [Key]
        public int id { get; set; }
        public decimal break_limit_down_times { get; set; }
        public decimal break_limit_up_times { get; set; }
        public decimal buy_lock_volume_ratio { get; set; }
        public decimal change_percent { get; set; }
        public decimal first_break_limit_down { get; set; }
        public decimal first_break_limit_up { get; set; }
        public decimal first_limit_down { get; set; }
        public decimal first_limit_up { get; set; }
        public bool is_new_stock { get; set; }
        public decimal issue_price { get; set; }
        public decimal last_break_limit_down { get; set; }
        public decimal last_break_limit_up { get; set; }
        public decimal last_limit_down { get; set; }
        public decimal last_limit_up { get; set; }
        public decimal limit_down_days { get; set; }
        public string limit_timeline { get; set; }
        public decimal limit_up_days { get; set; }
        public decimal listed_date { get; set; }
        public decimal m_days_n_boards_boards { get; set; }
        public decimal m_days_n_boards_days { get; set; }
        public decimal mtm { get; set; }
        public decimal nearly_new_acc_pcp { get; set; }
        public decimal nearly_new_break_days { get; set; }
        public decimal new_stock_acc_pcp { get; set; }
        public decimal new_stock_break_limit_up { get; set; }
        public decimal new_stock_limit_up_days { get; set; }
        public decimal new_stock_limit_up_price_before_broken { get; set; }
        public decimal non_restricted_capital { get; set; }
        public decimal price { get; set; }
        public decimal sell_lock_volume_ratio { get; set; }
        public string stock_chi_name { get; set; }
        public decimal stock_type { get; set; }
        public string surge_reason { get; set; }
        public string symbol { get; set; }
        public decimal total_capital { get; set; }
        public decimal turnover_ratio { get; set; }
        public decimal volume_bias_ratio { get; set; }
        public decimal yesterday_break_limit_up_times { get; set; }
        public decimal yesterday_first_limit_up { get; set; }
        public decimal yesterday_last_limit_up { get; set; }
        public decimal yesterday_limit_down_days { get; set; }
        public decimal yesterday_limit_up_days { get; set; }
    }

    public class APIPoolDetail
    {
        public decimal limit_up_days { get; set; }
        public string symbol { get; set; }

        public string stock_chi_name { get; set; }

        public string thatDate { get; set; }

        public string strWeek { get; set; }
    }

    public class APIPoolDetailWithoutST
    {
        public decimal limit_up_days { get; set; }
        public string symbol { get; set; }

        public string stock_chi_name { get; set; }

        public string thatDate { get; set; }

        public string strWeek { get; set; }
    }
}
