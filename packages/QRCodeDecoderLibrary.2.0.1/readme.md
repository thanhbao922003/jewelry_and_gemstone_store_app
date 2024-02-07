# QRCodeDecoder


The QR Code Decoder library allows your program to extract (decode) QR Code image from an image file. The encoder solution targets .NET framework (net462). It is writen in C# for Visual Studio.



The full article is published at CodeProject website. <a href="https://www.codeproject.com/Articles/1250071/QR-Code-Encoder-and-Decoder-NET-class-library-writ">QR Code Encoder and Decoder .NET(Framework, Standard, Core) Class Library Written in C# (Ver. 2.0.0)</a>



The solution is made of three projects:



<ul>

<li><strong><code>QRCodeDecoderLibrary</code>:</strong> A library exposing QR Code decoder. Visual studio will produce one DLL for .NET framework (net462). </li>

<li><strong><code>QRCodeDecoderDemo</code>:</strong> A demo program showing how to decode Bitmap image of a QR Code and retrieve the <code>string</code> or byte array</li>

<li><strong><code>QRCodeVideoDecoder</code>:</strong> A demo program combining QR Code decoder and video camera image capture. The video camera software is based on Direct Show Library. Please note section QR Code Video Decoder at the bottom of this article.</li>

</ul>

