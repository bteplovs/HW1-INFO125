using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Bte32_Homework1._0
{
    public partial class Form1 : Form
    {
        // GLOBAL VARS

        const Single PRICETWO = 15;
        const Single PRICEFOUR = 20;
        const Single PRICESIX = 30;

        const float PRICEBRONZEMEMBER = 1.1F;
        const float PRICESILVERMEMBER = 1.05F;
        const float PRICEGOLDMEMBER = 1.00F;

        const float PRICETWOMEALS = 0.97F;
        const float PRICEALLMEALS = 0.95F;
        const float PRICEONEMEAL = 1.00F;

        const float TAX = 1.08F;

        // #########################

        float priceBeforeTax = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            // Calls the clear help function
            Clear();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            // Closes application
            this.Close();
        }

        private void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            // Call clear to reset window after an order is placed
            Clear();
            MessageBox.Show("Order Placed!");
        }

        private void btnCalcPrice_Click(object sender, EventArgs e)
        {

            /* 
            
            Validation logic implemented upon btnCalcPrice click 
            
            If user incorreclty enters information this is where it will be caught and message box will appear

            */

            ////////////////////////////////////////////////////////////////////////////////////

            //If no meal type is selected show message, return None

            if (!chkbxTwoPerson.Checked && !chkbxFourPerson.Checked && !chkbxSixPerson.Checked)
            {
                MessageBox.Show("Error: Please select at least one meal type.");
                return;
            }

            //If a meal is selected but a quantity is not specified, show Message, return None

            if (chkbxTwoPerson.Checked && string.IsNullOrEmpty(cmbboxTwoPerson.Text))
            {
                MessageBox.Show("Error: Please enter a quantity for Two Person meal.");
                return;
            }
            if (chkbxFourPerson.Checked && string.IsNullOrEmpty(cmbboxFourPerson.Text))
            {
                MessageBox.Show("Error: Please enter a quantity for Four Person meal.");
                return;
            }
            if (chkbxSixPerson.Checked && string.IsNullOrEmpty(cmbboxSixPerson.Text))
            {
                MessageBox.Show("Error: Please enter a quantity for Six Person meal.");
                return;
            }

            //If no membership is selected show Message, return None

            if (!rbtnBronze.Checked && !rbtnSilver.Checked && !rbtnGold.Checked)
            {
                MessageBox.Show("Error: Please select a membership type.");
                return;
            }

            ////////////////////////////////////////////////////////////////////////////////////

            // Final value calculations

            float totalBeforeTax = Total();
            if (totalBeforeTax == 0)
            {
                return; // If Total() returns 0, KILL the function 
            } 

            float discount = SelectedMoreThanOne();
            float totalAndDiscount = totalBeforeTax * discount;
            float totalAndTax = totalAndDiscount * TAX;

            txtboxDiscountAmount.Text = (totalBeforeTax * (1 - discount)).ToString("C");
            txtboxTaxAmount.Text = (totalAndDiscount * (TAX - 1)).ToString("C");
            txtboxTotalPrice.Text = totalAndTax.ToString("C");
        }

        private void Clear()
        {
            // Clears all parameters in case the customer makes a mistake while inputting data

            // Reset Meals
            chkbxTwoPerson.Checked = false;
            chkbxFourPerson.Checked = false;
            chkbxSixPerson.Checked = false;
            // Reset quantity
            cmbboxTwoPerson.Text = "";
            cmbboxFourPerson.Text = "";
            cmbboxSixPerson.Text = "";
            // Reset Membership
            rbtnBronze.Checked = false;
            rbtnSilver.Checked = false;
            rbtnGold.Checked = false;
            //Reset read-only textboxes
            txtboxDiscountAmount.Text = "";
            txtboxTaxAmount.Text = "";
            txtboxTotalPrice.Text = "";
        }

        Single[] QuantityOrdered()
        {
            /*
            
            If a meal is checked and its corresponding combobox is a valid input (not empty) append the quantity to its
            corresponding index for each meal. if a meal is not ticked or input is not valid if statement fails and the program moves on

            index 0 (1) = Two meal qty
            index 1 (2) = Four meal qty
            index 2 (3) = Six meal qty

            and return the list

            */

            Single[] quantityList = { 0, 0, 0 };

            //checks if input is valid in the comboboxes, parses type, assigns value to new Single var "quantity{Num}People"

            bool isValidTwo = Single.TryParse(cmbboxTwoPerson.Text, out Single quantityTwoPeople);
            bool isValidFour = Single.TryParse(cmbboxFourPerson.Text, out Single quantityFourPeople);
            bool isValidSix = Single.TryParse(cmbboxSixPerson.Text, out Single quantitySixPeople);

            if (chkbxTwoPerson.Checked && isValidTwo)
            {
                quantityList[0] = quantityTwoPeople;
            }

            if (chkbxFourPerson.Checked && isValidFour)
            {
                quantityList[1] = quantityFourPeople;
            }

            if (chkbxSixPerson.Checked && isValidSix)
            {
                quantityList[2] = quantitySixPeople;
            }

            return quantityList;
        }

        Single[] PriceOfOrdered()
        {
            /*
            
            If a meal is checked and its corresponding combobox is a valid input (not empty) append the 
            (quantity * price per unit) to its corresponding index for each meal. This results in each index 
            holding the total price for each meal and its qty ordered. if a meal is not ticked or 
            input is not valid, if statement fails and the program moves on

            index 0 (1) = Two meal price
            index 1 (2) = Four meal price
            index 2 (3) = Six meal price

            and return the list

            */

            Single[] priceList = { 0, 0, 0 };


            //checks if input is valid in the comboboxes, parses type, assigns value to new Single var "quantity{Num}People"

            bool isValidTwo = Single.TryParse(cmbboxTwoPerson.Text, out Single quantityTwoPeople);
            bool isValidFour = Single.TryParse(cmbboxFourPerson.Text, out Single quantityFourPeople);
            bool isValidSix = Single.TryParse(cmbboxSixPerson.Text, out Single quantitySixPeople);

            //If meal is checked and combobox is valid (has num)

            if (chkbxTwoPerson.Checked && isValidTwo)
            {
                priceList[0] = PRICETWO * quantityTwoPeople;
            }

            if (chkbxFourPerson.Checked && isValidFour)
            {
                priceList[1] = PRICEFOUR * quantityFourPeople;
            }

            if (chkbxSixPerson.Checked && isValidSix)
            {
                priceList[2] = PRICESIX * quantitySixPeople;
            }

            return priceList;
        }

        float SelectedMoreThanOne()
        {
            /* 
            
            Returns discount if more than 1, upto 3 meals have been selected. Using a for loop to iterate through the quantity
            if the quantity is greater than 0 add a meal to sum total amount of unique meals

            */

            Single[] orderedMeals = QuantityOrdered();
            Single sum = 0;
            for (int i = 0; i < orderedMeals.Length; i++)
            {
                if (orderedMeals[i] > 0)
                {
                    sum++;
                }
            }

            //Sum (unique meals) value determines the discount applied

            if (sum == 3)
            {
                return PRICEALLMEALS;
            }
            else if (sum == 2)
            {
                return PRICETWOMEALS;
            }
            else
            {
                return PRICEONEMEAL;
            }
        }

        float Membership()
        {
            //Returns membership by checking which membership radio box is ticked

            if (rbtnBronze.Checked)
            {
                return PRICEBRONZEMEMBER;
            }
            if (rbtnSilver.Checked)
            {
                return PRICESILVERMEMBER;
            }
            else 
            { 
                return PRICEGOLDMEMBER;
            }
        }

        float Total()
        {
            Single totalPrice = PriceOfOrdered().Sum();
            Single totalOrdered = QuantityOrdered().Sum();

            // If more than 10 units ordered return float 0 (nothing) and throw error else calculate the total price with membership applied

            if (totalOrdered > 10) 
            {
                MessageBox.Show("Error: Cannot order more than 10 units.");
                return 0;
            }
            else
            {
                return totalPrice * Membership();
            }
        }
    }
}
