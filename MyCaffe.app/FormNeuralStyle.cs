﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyCaffe.app
{
    public partial class FormNeuralStyle : Form
    {
        NeuralStyleInfo m_info = new NeuralStyleInfo(null, null, 1000, "vgg19", "LBFGS", 1.0, null, 0, 0);

        public FormNeuralStyle(string strStyleFile, string strContentFile, int nIterations, string strModelName, string strSolverType, double dfLr, string strResultPath, int nIntermediateIterations, double dfTvLoss)
        {
            m_info = new NeuralStyleInfo(strStyleFile, strContentFile, nIterations, strModelName, strSolverType, dfLr, strResultPath, nIntermediateIterations, dfTvLoss);
            InitializeComponent();
        }

        public NeuralStyleInfo Info
        {
            get { return m_info; }
        }

        private void FormNeuralStyle_Load(object sender, EventArgs e)
        {
            cmbModel.SelectedIndex = 0;
            cmbSolver.SelectedIndex = 0;

            if (cmbModel.Items.Contains(m_info.ModelName))
                cmbModel.SelectedItem = m_info.ModelName;

            if (cmbSolver.Items.Contains(m_info.SolverType))
                cmbSolver.SelectedItem = m_info.SolverType;

            edtIterations.Text = m_info.Iterations.ToString();
            edtLearningRate.Text = m_info.LearningRate.ToString();

            if (m_info.IntermediateIterations == 0)
            {
                edtIntermediateIterations.Text = "0";
                chkIntermediateOutput.Checked = false;
            }
            else
            {
                edtIntermediateIterations.Text = m_info.IntermediateIterations.ToString();
                chkIntermediateOutput.Checked = true;
            }

            edtResultPath.Text = m_info.ResultPath;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string strModel = cmbModel.Text;
            string strSolverType = cmbSolver.Text;
            int nIterations;
            int nIntermediateIterations = 0;
            double dfLr;
            double dfTvLoss = 0;

            if (!int.TryParse(edtIterations.Text, out nIterations) || nIterations < 1)
            {
                MessageBox.Show("The 'Iterations' value is invalid - enter a positive integer greater than one.", "Invalid Iterations", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                edtIterations.Focus();
                return;
            }

            if (!double.TryParse(edtLearningRate.Text, out dfLr) || dfLr <= 0)
            {
                MessageBox.Show("The 'Learning Rate' value is invalid - enter a positive real value greater than one.", "Invalid Learning Rate", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                edtLearningRate.Focus();
                return;
            }

            if (!Directory.Exists(edtResultPath.Text))
            {
                MessageBox.Show("The 'Result Path' is invalid, please enter the path to an existing folder.", "Invalid Result Path", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                btnBrowseResultPath.Focus();
                return;
            }

            if (chkIntermediateOutput.Checked)
            {
                if (!int.TryParse(edtIntermediateIterations.Text, out nIntermediateIterations) || nIntermediateIterations < 0 || nIntermediateIterations > nIterations)
                {
                    MessageBox.Show("The 'Intermediate Iterations' value is invalid - enter a positive integer within the range [1," + nIterations.ToString() + "].", "Invalid Intermediate Iterations", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DialogResult = DialogResult.None;
                    edtIntermediateIterations.Focus();
                    return;
                }
            }

            if (chkEnableTvLoss.Checked)
            {
                if (!double.TryParse(edtTvLoss.Text, out dfTvLoss) || dfTvLoss < 0 || dfTvLoss > 0.1)
                {
                    MessageBox.Show("The 'TV-Loss' value is invalid - enter a real value within the range [0,0.1].", "Invalid TV-Loss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DialogResult = DialogResult.None;
                    edtTvLoss.Focus();
                    return;
                }
            }

            if (!File.Exists(edtContentImageFile.Text))
            {
                MessageBox.Show("Could not find the content file '" + edtContentImageFile.Text + "'!", "Invalid Content File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                btnBrowseContent.Focus();
                return;
            }

            if (!File.Exists(edtStyleImageFile.Text))
            {
                MessageBox.Show("Could not find the style file '" + edtStyleImageFile.Text + "'!", "Invalid Style File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                btnBrowseStyle.Focus();
                return;
            }

            m_info = new NeuralStyleInfo(edtStyleImageFile.Text, edtContentImageFile.Text, nIterations, strModel.ToLower(), strSolverType, dfLr, edtResultPath.Text, nIntermediateIterations, dfTvLoss);
        }

        private void cmbSolver_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbSolver.Text)
            {
                case "LBFGS":
                    edtLearningRate.Text = "1.5";
                    edtIterations.Text = "1000";
                    break;

                case "ADAM":
                    edtLearningRate.Text = "1.0";
                    edtIterations.Text = "3000";
                    break;

                case "RMSPROP":
                    edtLearningRate.Text = "1.0";
                    edtIterations.Text = "3000";
                    break;

                case "SGD":
                    edtLearningRate.Text = "1.0";
                    edtIterations.Text = "3000";
                    break;
            }
        }

        private void chkIntermediateOutput_CheckedChanged(object sender, EventArgs e)
        {
            edtIntermediateIterations.Enabled = chkIntermediateOutput.Checked;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = m_info.ResultPath;

            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK)
                return;

            edtResultPath.Text = folderBrowserDialog1.SelectedPath;
        }

        private void chkEnableTvLoss_CheckedChanged(object sender, EventArgs e)
        {
            edtTvLoss.Enabled = chkEnableTvLoss.Checked;
        }

        private void btnBrowseStyle_Click(object sender, EventArgs e)
        {
            if (openFileDialogStyle.ShowDialog() == DialogResult.OK)
                edtStyleImageFile.Text = openFileDialogStyle.FileName;
        }

        private void btnBrowseContent_Click(object sender, EventArgs e)
        {
            if (openFileDialogContent.ShowDialog() == DialogResult.OK)
                edtContentImageFile.Text = openFileDialogContent.FileName;
        }
    }

    public class NeuralStyleInfo
    {
        string m_strSolverType;
        string m_strModelName;
        int m_nIterations;
        double m_dfLearningRate;
        string m_strResultPath;
        int m_nIntermediateIterations = 0;
        double m_dfTVLoss = 0;
        string m_strContentImg;
        string m_strStyleImg;

        public NeuralStyleInfo(string strStyleImg, string strContentImg, int nIterations, string strModelName, string strSolverType, double dfLearningRate, string strResultPath, int nIntermediateIterations, double dfTvLoss)
        {
            m_strStyleImg = strStyleImg;
            m_strContentImg = strContentImg;
            m_dfTVLoss = dfTvLoss;
            m_nIterations = nIterations;
            m_strModelName = strModelName;
            m_strSolverType = strSolverType;
            m_dfLearningRate = dfLearningRate;
            m_strResultPath = strResultPath;
            m_nIntermediateIterations = nIntermediateIterations;
        }

        public string StyleImageFile
        {
            get { return m_strStyleImg; }
        }

        public string ContentImageFile
        {
            get { return m_strContentImg; }
        }

        public int Iterations
        {
            get { return m_nIterations; }
        }

        public int IntermediateIterations
        {
            get { return m_nIntermediateIterations; }
        }

        public string ResultPath
        {
            get { return m_strResultPath; }
        }

        public string SolverType
        {
            get { return m_strSolverType; }
        }

        public string ModelName
        {
            get { return m_strModelName; }
        }

        public double LearningRate
        {
            get { return m_dfLearningRate; }
        }

        public double TVLoss
        {
            get { return m_dfTVLoss; }
        }
    }
}
