using Core.Constants;
using Core.Entities;
using Core.Entities.Product;
using Ecom.Core.Entities.Product;
using System;

namespace Core.Entities.Animal
{
    public class FosterAnimal : BaseAnimal
    {
        public bool IsFostered => FostererId != null;

        public string? FostererId { get; set; }
        public AppUser? Fosterer { get; set; }

        public DateTime? FosterStartDate { get; set; }
        public DateTime? FosterEndDate { get; set; }

        public string? FosterNotes { get; set; }

        public FosterStatus Status { get; set; } = FosterStatus.Available;
        // Available, InFoster

        public bool IsUrgent { get; set; }

        public int? FosterDurationDays =>
            FosterStartDate.HasValue && FosterEndDate.HasValue
                ? (FosterEndDate.Value - FosterStartDate.Value).Days
                : null;
    }
}
