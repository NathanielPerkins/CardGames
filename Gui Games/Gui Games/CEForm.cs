﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shared_Game_Class_Library;
using GuiGames;
using Game_Class_Library;

namespace Gui_Games
{
    /// <summary>
    /// Contains the Form methods and variables to be used in conjunction
    /// with the Crazy Eights Game library for the Crazy Eights Game
    /// 
    /// Authors: Nathan Perkins
    /// Date: 01/06/2015
    /// </summary>
    public partial class CEForm : Form
    {
        PictureBox[] playerPBox; //picture boxes for player cards
        PictureBox[] computerPBox; //pictures boxes for computer cards

        //Strings required for instruction text
        string yourTurnText = "Your turn. Click to place\n a card";
        string dealText = "Click Deal to start the game \n ";
        string wrongCard = "Please select a Legal move to \n play";
        string canDrawText = "It is possible to make a move\n Please make a selection";
        public CEForm()
        {
            InitializeComponent();
            playerPBox = new PictureBox[13];
            computerPBox = new PictureBox[13];
            SetupDeck();
            InstructionText.Text = dealText;


        }
        /// <summary>
        /// Loads the back of card image to the deck as a setup for the game
        /// </summary>
        private void SetupDeck()
        {

            DeckPB.Image = Images.GetBackOfCardImage();
        }

        /// <summary>
        /// Refreshes all the images based on current hands, deck and discard piles
        /// </summary>
        private void Refresh()
        {
            UpdatePHand();
            DisplayPHand();
            UpdateCHand();
            DisplayCHand();
            UpdateDeck();
            UpdateDiscard();
        }

        /// <summary>
        /// Updates the pictureboxes for the player with images for current
        /// players hand
        /// </summary>
        private void UpdatePHand()
        {
            PictureBox pBox;
            Hand pHand = Crazy_Eights_Game.GetPlayerHand();
            int handSize = pHand.GetCount();
            for (int i = 0; i < handSize; i++)
            {
                Card card = pHand.GetCard(i);
                pBox = new PictureBox();
                pBox.SizeMode = PictureBoxSizeMode.AutoSize;
                pBox.Dock = DockStyle.Fill;
                pBox.Image = Images.GetCardImage(card);
                playerPBox[i] = pBox;
                playerPBox[i].Click += new EventHandler(PlayerPBox_Click);
                playerPBox[i].Tag = Crazy_Eights_Game.GetPlayerHand().GetCard(i);
            }

        }
        /// <summary>
        /// updates picture boxes for the computer based on the current
        /// computers hand
        /// </summary>
        private void UpdateCHand()
        {
            PictureBox pBox;
            Hand cHand = Crazy_Eights_Game.GetComputerHand();
            int handSize = cHand.GetCount();
            for (int i = 0; i < handSize; i++)
            {
                Card card = cHand.GetCard(i);
                pBox = new PictureBox();
                pBox.SizeMode = PictureBoxSizeMode.AutoSize;
                pBox.Dock = DockStyle.Fill;
                pBox.Image = Images.GetCardImage(card);
                computerPBox[i] = pBox;
            }
        }

        /// <summary>
        /// Loads and displays the pictureboxes into the computer table layout
        /// panel
        /// </summary>
        private void DisplayCHand()
        {
            CompPanel.Controls.Clear();
            Hand cHand = Crazy_Eights_Game.GetComputerHand();
            int handSize = cHand.GetCount();
            for (int i = 0; i < handSize; i++)
            {
                CompPanel.Controls.Add(computerPBox[i]);
            }
        }

        /// <summary>
        /// Loads and displays the pictureboxes into the player table layout
        /// panel
        /// </summary>
        private void DisplayPHand()
        {
            PlayerPanel.Controls.Clear();
            Hand pHand = Crazy_Eights_Game.GetPlayerHand();
            int handSize = pHand.GetCount();
            for (int i = 0; i < handSize; i++)
            {
                PlayerPanel.Controls.Add(playerPBox[i]);
            }
        }

        private void PlayerPBox_Click(object sender, EventArgs e)
        {
            //Get the selected card from the picture box
            int whichCard;
            PictureBox whichClicked = (PictureBox)sender;
            whichCard = Crazy_Eights_Game.WhichCard((Card)whichClicked.Tag); 
            Card Selected = Crazy_Eights_Game.GetPlayerHand().GetCard(whichCard);
            
            if (Crazy_Eights_Game.IsEight(Selected)) //If the selected card is an eight, load suit selection form
            {
                SuitSelection suitSelection = new SuitSelection();
                DialogResult result = suitSelection.ShowDialog();
                Card card = new Card();
                if (result == DialogResult.OK)
                {
                    card = suitSelection.GetSelection();
                }
                Crazy_Eights_Game.PlayerTurn(Selected,card);
                Crazy_Eights_Game.ComputerTurn();
                Refresh();
            }
            else
            {
                bool check = Crazy_Eights_Game.PlayerTurn(Selected);
                if (check) //
                {

                    Crazy_Eights_Game.ComputerTurn();
                    InstructionText.Text = yourTurnText;
                    Refresh();
                }
                else //If no card is played, change instruction text
                {
                    InstructionText.Text = wrongCard;
                }
            }
        }

        /// <summary>
        /// Updates the Discard deck and displays the new images
        /// </summary>
        private void UpdateDiscard()
        {
            Card card = Crazy_Eights_Game.GetTopOfDiscard();
            DiscardPB.Image = Images.GetCardImage(card);
            //check current top of discard pile and set picturebox to that card
        }

        /// <summary>
        /// Checks if the deck is out of cards and tells crazy eights class
        /// to swap if so
        /// </summary>
        private void UpdateDeck()
        {
            Crazy_Eights_Game.DeckDiscardSwap();
        }



        private void CEForm_Load(object sender, EventArgs e)
        {

        }


        private void DealBtn_Click(object sender, EventArgs e)
        {
            DeckPB.Enabled = true;
            Crazy_Eights_Game.SetupGame();
            DealBtn.Enabled = false;
            SortBtn.Enabled = true;
            Refresh();
            InstructionText.Text = yourTurnText;
        }

        private void SortBtn_Click(object sender, EventArgs e)
        {
            Crazy_Eights_Game.SortPlayerHand();
            Refresh();
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DeckPB_Click(object sender, EventArgs e)
        {
            bool canDraw = Crazy_Eights_Game.AddFromDeck();
            if (!canDraw)
            {
                InstructionText.Text = canDrawText;
            }
            Crazy_Eights_Game.DeckDiscardSwap();
            Refresh();
            MessageBox.Show(Crazy_Eights_Game.GetDeck().GetCount().ToString());
        }
    }
}