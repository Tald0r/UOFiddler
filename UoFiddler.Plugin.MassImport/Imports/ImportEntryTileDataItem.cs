// /***************************************************************************
//  *
//  * $Author: Turley
//  *
//  * "THE BEER-WARE LICENSE"
//  * As long as you retain this notice you can do whatever you want with
//  * this stuff. If we meet some day, and you think this stuff is worth it,
//  * you can buy me a beer in return.
//  *
//  ***************************************************************************/

using System;
using System.Collections.Generic;
using UoFiddler.Controls.Classes;

namespace UoFiddler.Plugin.MassImport.Imports
{
    public class ImportEntryTileDataItem : ImportEntry
    {
        private string[] _tiledata;

        public override int MaxIndex => Ultima.Art.GetMaxItemId();
        
        public override string Name => "TileDataItem";

        protected override void TestFile(ref string message)
        {
            if (!File.Contains(".csv"))
            {
                message += " Invalid File format";
                Valid = false;
            }
            else
            {
                Valid = GetTileDataInfo(File, ref message, ref _tiledata);
            }
        }

        public override void Import(bool direct, ref Dictionary<string, bool> changedClasses)
        {
            int dest = OutputIndex >= 0 ? OutputIndex : Index;

            // Classic item CSV has 44 columns; extended SA/HS has 75 columns.
            const int CLASSIC_ITEM_COLS = 44;
            const int EXTENDED_ITEM_COLS = 75;

            var data = _tiledata ?? Array.Empty<string>();
            int expected = Ultima.Art.IsUOAHS() ? EXTENDED_ITEM_COLS : CLASSIC_ITEM_COLS;

            if (data.Length < expected)
            {
                var pad = new string[expected];
                Array.Copy(data, pad, data.Length);
                for (int i = data.Length; i < expected; i++) pad[i] = "0";
                data = pad;
            }

            Ultima.TileData.ItemTable[dest].ReadData(data);

            if (!direct)
            {
                Options.ChangedUltimaClass["TileData"] = true;
            }

            // Items MUST fire change with +0x4000
            ControlEvents.FireTileDataChangeEvent(this, dest + 0x4000);
            changedClasses["TileData"] = true;
        }
    }
}