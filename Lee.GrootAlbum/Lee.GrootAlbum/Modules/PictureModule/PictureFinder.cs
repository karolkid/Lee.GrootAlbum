﻿using Azylee.Core.DataUtils.CollectionUtils;
using Azylee.Core.IOUtils.FileUtils;
using Azylee.Core.ThreadUtils.SleepUtils;
using Lee.GrootAlbum.Commons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lee.GrootAlbum.Modules.PictureModule
{
    public class PictureFinder
    {
        static CancellationTokenSource Token = new CancellationTokenSource();
        public static void Start()
        {
            Task.Factory.StartNew(() =>
            {
                while (!Token.IsCancellationRequested)
                {
                    List<string> files = FileTool.GetAllFile(R.Paths.Unsorted, new string[] { "*.jpg", "*.jpeg" });
                    if (ListTool.HasElements(files))
                    {
                        foreach (var file in files)
                        {
                            string ext = Path.GetExtension(file).ToLower();
                            if (ext.Contains("jpg") || ext.Contains("jpeg"))
                                PictureHandleQueue.Add(file);
                        }
                    }
                    Sleep.M(5);
                }
            });
        }

        /// <summary>
        /// 获取带GPS信息的照片
        /// </summary>
        /// <returns></returns>
        public static List<Pictures> GetPictures()
        {
            List<Pictures> list = new List<Pictures>();
            using (Muse db = new Muse("pictures"))
            {
                var _temp = db.Gets<Pictures>(x => x.GpsLongitude != 0 && x.GpsLatitude != 0, null).ToList();
                if (ListTool.HasElements(_temp)) list = _temp;
            }
            return list;
        }
    }
}
