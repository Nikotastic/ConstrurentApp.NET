public static class ToolsConfig
{
    public static object GetTools()
    {
        return new
        {
            functionDeclarations = new object[]
            {
                new {
                    name = "get_vehicle_by_id",
                    description = "Get vehicle info from database",
                    parameters = new {
                        type = "object",
                        properties = new {
                            id = new { type = "string" }
                        },
                        required = new[] { "id" }
                    }
                },

                new {
                    name = "get_product_by_id",
                    description = "Get product info from database",
                    parameters = new {
                        type = "object",
                        properties = new {
                            id = new { type = "string" }
                        },
                        required = new[] { "id" }
                    }
                },

                new {
                    name = "get_available_vehicles",
                    description = "Returns list of available vehicles by date",
                    parameters = new {
                        type = "object",
                        properties = new {
                            max_results = new { type = "number" }
                        },
                        required = Array.Empty<string>()
                    }
                },

                new {
                    name = "get_available_products",
                    description = "Returns list of available products",
                    parameters = new {
                        type = "object",
                        properties = new {
                            max_results = new { type = "number" }
                        },
                        required = Array.Empty<string>()
                    }
                },

                new {
                    name = "check_vehicle_availability",
                    description = "Checks if a vehicle is available for rent",
                    parameters = new {
                        type = "object",
                        properties = new {
                            vehicle_id = new { type = "string" },
                            start_date = new { type = "string" },
                            end_date = new { type = "string" }
                        },
                        required = new[] { "vehicle_id", "start_date", "end_date" }
                    }
                },

                new {
                    name = "analyze_expenses",
                    description = "Analyzes a list of expenses and provides a summary with totals by category, insights, and recommendations",
                    parameters = new {
                        type = "object",
                        properties = new {
                            expenses = new {
                                type = "array",
                                description = "Array of expense objects",
                                items = new {
                                    type = "object",
                                    properties = new {
                                        fecha = new { type = "string", description = "Date in YYYY-MM-DD format" },
                                        categoria = new { type = "string", description = "Expense category" },
                                        descripcion = new { type = "string", description = "Expense description" },
                                        monto = new { type = "number", description = "Amount spent" }
                                    }
                                }
                            }
                        },
                        required = new[] { "expenses" }
                    }
                }
            }
        };
    }
}

