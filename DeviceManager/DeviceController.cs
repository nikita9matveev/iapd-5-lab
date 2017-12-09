using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager
{
    class DeviceController
    {
        private const string SelectRequest = "SELECT * FROM Win32_PNPEntity";
        private List<Device> devices;
        public List<Device> Devices
        {
            get
            {
                return devices;
            }
            set
            {
                devices = value;
            }
        }

        public DeviceController()
        {
            this.devices = new List<Device>();
            var devices = new ManagementObjectSearcher(SelectRequest);
            
            foreach (ManagementObject device in devices.Get())
            {
                List<SysFile> sysFiles = new List<SysFile>();
                foreach (var sys in device.GetRelated("Win32_SystemDriver"))
                {
                    sysFiles.Add(new SysFile()
                    {
                        PathName = sys["PathName"].ToString(),
                        Description = sys["Description"].ToString()
                    });
                }
                this.devices.Add(new Device
                {
                    Name = device["Name"] != null ? device["Name"].ToString() : "",
                    ClassGuid = device["ClassGuid"] != null ? device["ClassGuid"].ToString() : "",
                    HardwareID = device["HardwareID"] != null ? (string[])device["HardwareID"] : null,
                    Manufacturer = device["Manufacturer"] != null ? device["Manufacturer"].ToString() : "",
                    DeviceID = device["DeviceID"] != null ? device["DeviceID"].ToString() : "",
                    SysFiles = sysFiles,
                    State = device["Status"].ToString() == "OK"
                });
            }
        }

        public void DisableDevice(Device _device)
        {
            var device = new ManagementObjectSearcher(SelectRequest).Get()
                .OfType<ManagementObject>()
                .FirstOrDefault(x => x.Properties["DeviceID"].Value.ToString().Equals(_device.DeviceID));
            device.InvokeMethod("Disable", new object[] { false }); //error
        }

        public void EnableDevice(Device _device)
        {
            var device = new ManagementObjectSearcher(SelectRequest).Get()
                 .OfType<ManagementObject>()
                 .FirstOrDefault(x => x["DeviceID"].ToString().Equals(_device.DeviceID));
                device.InvokeMethod("Enable", new object[] { false }); //error
        }
    }
}
