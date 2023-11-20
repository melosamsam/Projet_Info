using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

namespace ProjetInfoSaloméSuvetaa
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            MyImage image1 = new MyImage("./Test.bmp");
            Assert.AreEqual(image1.Taillefichier,1254);
            Assert.AreEqual(image1.Height, 20);
            Assert.AreEqual(image1.Width, 20);
            Assert.AreEqual(image1.Nbbitpcolor, 24);
            Assert.AreEqual(image1.Typeimage, "bmp");
        }
        [TestMethod]
        public void TestQr()
        {
            MyImage qr = new MyImage("./Test.bmp");
            qr.QRCode("hello world",21);
            Assert.AreEqual("hello world", qr.LectureQR(qr));
        }
        [TestMethod]
        public void Noiretblanc()
        {
            MyImage image = new MyImage("./coco.bmp");
            image.Noir_et_blanc();
            bool test = true;
            for (int i = 0; i < image.Image.GetLength(0); i++)
            {
                for (int j = 0; j < image.Image.GetLength(1); j++)
                {
                    if (image.Image[i, j].R != 255 && image.Image[i, j].R != 0 && image.Image[i, j].B != 255 && image.Image[i, j].B != 0 && image.Image[i, j].G != 255 && image.Image[i, j].B != 0)
                    {
                        test = false;
                    }
                }
            }
            Assert.AreEqual(test, true);
        }
        [TestMethod]
        public void TestConvertirBinaireToEntier()
        {
            MyImage image = new MyImage("./coco.bmp");
            int[] tab1 = { 0, 0, 1, 1, 0, 1, 0 };
            Assert.AreEqual(image.ConversionBinaireToEntier(tab1), 26);
            int[] tab2 = { 1, 1, 1, 0, 1, 1, 0, 1, 1 };
            Assert.AreEqual(image.ConversionBinaireToEntier(tab2), 475);
            

        }
        [TestMethod]
        public void TestNuancedegris()
        {
            MyImage image = new MyImage("./coco.bmp");
            bool test= true;
            image.Nuances_De_Gris();
            for (int i = 0; i < image.Image.GetLength(0); i++)
            {
                for (int j = 0; j < image.Image.GetLength(1); j++)
                {
                    if (image.Image[i, j].R !=  image.Image[i, j].B  && image.Image[i, j].R != image.Image[i, j].G && image.Image[i, j].G != image.Image[i, j].B)
                    {
                        test = false;
                    }
                }
            }
        }
    }
}
