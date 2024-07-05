using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace YourNamespace
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, List<string>> categoryIndustriesMap;

        public MainWindow()
        {
            InitializeComponent();
            InitializeCategoryIndustriesMap();
            LoadZones();
            LoadCategories();
        }

        private void InitializeCategoryIndustriesMap()
        {
            // Initialize the dictionary mapping categories to industries
            categoryIndustriesMap = new Dictionary<string, List<string>>
            {
                { "A", new List<string>
                    {
                        "Chlor-Alkali (Mercury Cell)",
                        "Chlor-Alkali (Diaphram Cell)",
                        "Metal finishing and electroplating",
                        "Nitrogenous fertilizer",
                        "Phosphate fertilizer",
                        "Pulp and paper",
                        "Pesticides formulation",
                        "Petroleum refining",
                        "Steel industry",
                        "Synthetic fiber",
                        "Tanning and leather finishing",
                        "Textile processing",
                        "Pigments and dyes",
                        "Thermal Power Plants (Oil Fired and Coal Fired)",
                        "Rubber products",
                        "Paints, Varnishes and Lacquers"
                    }
                },
                { "B", new List<string>
                    {
                        "Dairy industry",
                        "Fruit and vegetable processing",
                        "Glass manufacturing",
                        "Sugar",
                        "Detergent",
                        "Photographic",
                        "Glue manufacture",
                        "Oil and Gas exploration",
                        "Thermal Power Plants (Gas Fired)",
                        "Vegetable oil and Ghee mills",
                        "Woolen mills",
                        "Plastic materials and products",
                        "Wood and cork products",
                        "Any other industry to be specified by Federal or Provincial Agency"
                    }
                },
                { "C", new List<string>
                    {
                        "Pharmaceutical (Formulation) Industry",
                        "Marble Crushing",
                        "Cement",
                        "Any other industry to be specified by Federal or Provincial Agency"
                    }
                }
            };
        }

        private void LoadZones()
        {
            var zones = new List<Zone>
            {
                new Zone { Id = 1, Name = "I-9" },
                new Zone { Id = 2, Name = "I-10" },
                new Zone { Id = 3, Name = "Humak" },
                new Zone { Id = 4, Name = "Tarnol" }
            };
            ZoneComboBox.ItemsSource = zones;
            ZoneComboBox.DisplayMemberPath = "Name";
            ZoneComboBox.SelectedValuePath = "Id";
        }

        private void LoadCategories()
        {
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "A" },
                new Category { Id = 2, Name = "B" },
                new Category { Id = 3, Name = "C" }
            };

            // Clear existing items
            CategoryComboBox.Items.Clear();

            // Set the ItemsSource
            CategoryComboBox.ItemsSource = categories;

            CategoryComboBox.DisplayMemberPath = "Name";
            CategoryComboBox.SelectedValuePath = "Id";
        }

        // Method to handle adding industry to respective category
        private void AddIndustryToCategory(string industryName, string category)
        {
            switch (category)
            {
                case "A":
                    if (!categoryIndustriesMap["A"].Contains(industryName))
                        categoryIndustriesMap["A"].Add(industryName);
                    break;
                case "B":
                    if (!categoryIndustriesMap["B"].Contains(industryName))
                        categoryIndustriesMap["B"].Add(industryName);
                    break;
                case "C":
                    if (!categoryIndustriesMap["C"].Contains(industryName))
                        categoryIndustriesMap["C"].Add(industryName);
                    break;
                default:
                    throw new ArgumentException("Invalid category.");
            }
        }

        // Event handler for Add Industry button click
        private void OnAddIndustryClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string newIndustryName = NewIndustryTextBox.Text.Trim();
                string selectedCategory = (string)((ComboBoxItem)NewIndustryCategoryComboBox.SelectedItem).Content;

                if (string.IsNullOrEmpty(newIndustryName))
                {
                    MessageBox.Show("Please enter a valid industry name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                AddIndustryToCategory(newIndustryName, selectedCategory);

                MessageBox.Show($"Industry '{newIndustryName}' added to Category {selectedCategory}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Refresh the IndustriesComboBox
                RefreshIndustriesComboBox(selectedCategory);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding industry: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Method to refresh IndustriesComboBox based on selected category
        private void RefreshIndustriesComboBox(string selectedCategory)
        {
            IndustriesComboBox.Items.Clear();

            if (categoryIndustriesMap.ContainsKey(selectedCategory))
            {
                foreach (var industry in categoryIndustriesMap[selectedCategory])
                {
                    IndustriesComboBox.Items.Add(industry);
                }
            }
        }

        // Event handler for CategoryComboBox selection change
        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Clear existing items
            IndustriesComboBox.Items.Clear();

            // Get selected category
            var selectedCategory = (Category)CategoryComboBox.SelectedItem;
            if (selectedCategory != null)
            {
                // Get industries for selected category
                if (categoryIndustriesMap.ContainsKey(selectedCategory.Name))
                {
                    var industries = categoryIndustriesMap[selectedCategory.Name];
                    foreach (var industry in industries)
                    {
                        IndustriesComboBox.Items.Add(industry);
                    }
                }
            }
        }

        private void OnGenerateReportClick(object sender, RoutedEventArgs e)
        {
            var selectedZone = (Zone)ZoneComboBox.SelectedItem;
            var selectedCategory = (Category)CategoryComboBox.SelectedItem;
            var selectedIndustry = (string)IndustriesComboBox.SelectedItem;

            var industryAddress = new IndustryAddress
            {
                Building = BuildingTextBox.Text,
                Street = StreetTextBox.Text,
                Sector = SectorTextBox.Text,
                Contact = ContactTextBox.Text
            };

            var emissions = new Emission
            {
                Air = AirEmissionTextBox.Text,
                Water = new WaterEmission
                {
                    Drinking = DrinkingWaterTextBox.Text,
                    Sewage = SewageWaterTextBox.Text
                },
                Solid = new SolidEmission
                {
                    Hospital = HospitalWasteTextBox.Text,
                    Surface = SurfaceWasteTextBox.Text,
                    Sludge = SludgeTextBox.Text
                }
            };

            Save(selectedZone, selectedCategory, selectedIndustry, industryAddress, emissions);
        }

        private void Save(Zone zone, Category category, string industry, IndustryAddress address, Emission emissions)
        {
            try
            {
                string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "IndustryReport.pdf");

                // Create a new MigraDoc document
                Document document = new Document();
                Section section = document.AddSection();

                // Add content to the PDF using MigraDoc
                Paragraph title = section.AddParagraph("Industry Emissions Report");
                title.Format.Font.Size = 14;
                title.Format.Font.Bold = true;

                section.AddParagraph($"Zone: {zone.Name}");
                section.AddParagraph($"Category: {category.Name}");
                section.AddParagraph($"Industry: {industry}");
                section.AddParagraph($"Building: {address.Building}");
                section.AddParagraph($"Street: {address.Street}");
                section.AddParagraph($"Sector: {address.Sector}");
                section.AddParagraph($"Contact: {address.Contact}");
                section.AddParagraph();

                Paragraph emissionsTitle = section.AddParagraph("Emissions:");
                emissionsTitle.Format.Font.Bold = true;

                section.AddParagraph($"  Air Emissions: {emissions.Air}");
                section.AddParagraph($"  Water Emissions:");
                section.AddParagraph($"    Drinking: {emissions.Water.Drinking}");
                section.AddParagraph($"    Sewage: {emissions.Water.Sewage}");
                section.AddParagraph($"  Solid Emissions:");
                section.AddParagraph($"    Hospital: {emissions.Solid.Hospital}");
                section.AddParagraph($"    Surface: {emissions.Solid.Surface}");
                section.AddParagraph($"    Sludge: {emissions.Solid.Sludge}");

                // Save the document to a file
                PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer();
                pdfRenderer.Document = document;
                pdfRenderer.RenderDocument();
                pdfRenderer.PdfDocument.Save(outputPath);

                MessageBox.Show($"PDF report generated successfully: {outputPath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating PDF report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class Zone
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class IndustryAddress
    {
        public string Building { get; set; }
        public string Street { get; set; }
        public string Sector { get; set; }
        public string Contact { get; set; }
    }

    public class Emission
    {
        public string Air { get; set; }
        public WaterEmission Water { get; set; }
        public SolidEmission Solid { get; set; }
    }

    public class WaterEmission
    {
        public string Drinking { get; set; }
        public string Sewage { get; set; }
    }

    public class SolidEmission
    {
        public string Hospital { get; set; }
        public string Surface { get; set; }
        public string Sludge { get; set; }
    }
}
