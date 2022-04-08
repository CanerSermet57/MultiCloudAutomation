﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util;
using Google.Apis.Services;
using Google.Apis.Compute.v1;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon;
using Newtonsoft.Json.Linq;
using static MultiCloudAutomation.Request;

namespace MultiCloudAutomation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
        }

        //Beginning Variables
        ResponseClass task;
        
        
        public async Task<ResponseClass> login()
        {
            AWS.LoginBody loginbody = new AWS.LoginBody(textBox1.Text, textBox2.Text);
            string jsonbody = JsonConvert.SerializeObject(loginbody);
            task = await Request.PostRequestAsync("aws/setCredential", jsonbody);
            return task;
        }
        
        public async Task<ResponseClass> getAllInstance(string region)
        {
            AWS.getAllInstanceBody body = new AWS.getAllInstanceBody(region);
            string jsonbody = JsonConvert.SerializeObject(body);
            task = await Request.PostRequestAsync("aws/instance/getAllInstance", jsonbody);
            return task;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            ResponseClass loginList = await login();
            MessageBox.Show(loginList.Content);
        }

        

        private async void button1_Click(object sender, EventArgs e)
        {            
            ResponseClass getallinstances = await getAllInstance("us-east-1");

            List<AWS.InstanceDataGridView> instanceDataGridViewList = new List<AWS.InstanceDataGridView>();
            if (getallinstances.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string jsonbody = JsonConvert.SerializeObject(getallinstances);
                JArray json_data = (JArray)JsonConvert.DeserializeObject(task.Content);
                foreach (var item in json_data)
                {
                    AWS.InstanceDataGridView instance = new AWS.InstanceDataGridView();

                    instance.InstanceId = item["instances"][0]["instanceId"].ToString();
                    instance.InstanceState = item["instances"][0]["state"]["name"]["value"].ToString();
                    instance.InstanceType = item["instances"][0]["instanceType"]["value"].ToString();
                    instance.AvailabilityZone = item["instances"][0]["placement"]["availabilityZone"].ToString();
                    instance.PublicIPv4DNS = item["instances"][0]["publicDnsName"].ToString();
                    instance.PublicIPv4Address = item["instances"][0]["publicIpAddress"].ToString();
                    instance.LaunchTime = item["instances"][0]["launchTime"].ToString();

                    instanceDataGridViewList.Add(instance);
                }
                dataGridView1.DataSource = instanceDataGridViewList;
            }
            else if (getallinstances.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                MessageBox.Show("Unauthorized Please First Login");
            }            
        }
    }
}