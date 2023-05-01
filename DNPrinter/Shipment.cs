using System.Collections.Generic;

namespace DNPrinter
{

    public class Shipment
    {
        List<Pallet> _Pallets = new List<Pallet>();
        public IList<Pallet> get => _Pallets;

        public void AddPallet(Pallet pallet)
        {

        }

        public void RemovePallet(Pallet pallet)
        {
        }
    }


    public class Pallet
    {
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public bool Rotated { get; set; }

        public List<Module> Modules;
    }


    public class Module
    {
        public int ModId { get; private set; }
        public string CustId { get; }
        public string DeliveryNoteId { get; }
        public string Serial { get; }
        public string Desc { get; }
        public string DeliveryNoteFile { get; }
    }


    public class ModuleLibraryItem
    {
        public int ModId { get; private set; }
        public string Desc { get; private set; }
        public string CustId { get; private set; }
    }


    public class ModuleLibrary
    {
        public List<ModuleLibraryItem> Items;
    }
}
