namespace Firmness.Domain.Enums;
// Inventory movement types
public enum InventoryMovementType
{ 
    // Inventory entry (purchase, customer return)

    In = 0,
    // Inventory outflow (sale, return to supplier)
    Out = 1,
    
    // Inventory adjustment (correction, physical count)
    Adjustment = 2,
    
    // Transfer between warehouses
    Transfer = 3,
    
    // Damaged or lost product
    Loss = 4
}

