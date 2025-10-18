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
    public class ImportEntryTileDataLand : ImportEntry
    {
        private string[] _tiledata;

        public override string Name => "TileDataLand";

        protected override void TestFile(ref string message)
        {
            if (!File.Contains(".csv"))
            {
                message += " Invalid file format";
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

            // Normalize the CSV row length:
            // classic land CSV has 35 columns (0..34), extended SA/HS land CSV has 66 columns (0..65).
            const int CLASSIC_COLS = 35;  // includes ID at [0]
            const int EXTENDED_COLS = 66; // adds AlphaBlend..Unused32 (no Unused9 in current header order)

            var data = _tiledata ?? Array.Empty<string>();
            int expected = Ultima.Art.IsUOAHS() ? EXTENDED_COLS : CLASSIC_COLS;

            if (data.Length < expected)
            {
                // pad with zeros for any missing columns so TileData.ReadData can index safely
                var pad = new string[expected];
                Array.Copy(data, pad, data.Length);
                for (int i = data.Length; i < expected; i++) pad[i] = "0";
                data = pad;
            }

            // Import into destination slot
            Ultima.TileData.LandTable[dest].ReadData(data);

            if (!direct)
            {
                Options.ChangedUltimaClass["TileData"] = true;
            }
            
            ControlEvents.FireTileDataChangeEvent(this, dest);
            changedClasses["TileData"] = true;
        }
    }
}