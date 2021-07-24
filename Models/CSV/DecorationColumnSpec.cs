﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RainWorx.FrameWorx.DTO;

namespace RainWorx.FrameWorx.MVC.Models.CSV
{
    public class DecorationColumnSpec : ColumnSpecBase
    {
        private readonly int _code;

        public DecorationColumnSpec(int number, string name, string notes, string cultureCode, int code, string example)
            : base(number, name, CustomFieldType.Boolean, notes, false, cultureCode, example)
        {
            _code = code;
        }

        public override void Translate(Dictionary<string, string> input, ImportListing csvRow, bool commitIntent)
        {
            if (!string.IsNullOrEmpty(csvRow.ColumnData[Name]))
            {
                input.Add("decoration_" + _code, csvRow.ColumnData[Name]);
            }
            else
            {
                input.Add("decoration_" + _code, "False");
            }
        }
    }
}