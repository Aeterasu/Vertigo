using Godot;
using System;

namespace Vertigo
{
    // OMISSION - when a hazard leaves the screen, it will spawn gems
    public class Omission : Upgrade
    {
        [Export] private readonly int _initialGems = 1;
        [Export] private readonly int _gemsPerStack = 1;

        private HazardSpawner _hazardSpawner;

        public int TotalGemCount { get; private set; } = 1;

        public override void _Ready()
        {
            base._Ready();
            
            if (!Game.Instance.IsValid())
            {
                return;
            }

            TotalGemCount += _initialGems;

            _hazardSpawner = Game.Instance.GetNodeOrNull<HazardSpawner>("%HazardSpawner");

            if (_hazardSpawner.IsValid())
            {
                _hazardSpawner.OnHazardLeftThePlayArea += OnHazardLeftThePlayArea;
            }
        }

        public override void AddStack()
        {
            base.AddStack();

            TotalGemCount += _gemsPerStack;
        }

        private void OnHazardLeftThePlayArea(Hazard hazard)
        {
            CurrencyGenerator currencyGenerator = hazard.GetNodeOrNull<CurrencyGenerator>("%CurrencyGenerator");

            if (currencyGenerator.IsValid())
            {
                currencyGenerator.GenerateByOmission(TotalGemCount);
            }
        }
    }
}
