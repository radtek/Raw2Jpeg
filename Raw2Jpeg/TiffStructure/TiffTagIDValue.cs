using System;
using System.Collections.Generic;
using System.Text;

namespace Raw2Jpeg.TiffStructure
{
        public static class TiffTagIDValue
        {

            static Dictionary<int, string> _value = new Dictionary<int, string>
        {
            {254,"NewSubfileType"},
{255,"SubfileType"},
{256,"ImageWidth"},
{257,"ImageLength"},
{258,"BitsPerSample"},
{259,"Compression"},
{262,"PhotometricInterpretation"},
{263,"Threshholding"},
{264,"CellWidth"},
{265,"CellLength"},
{266,"FillOrder"},
{270,"ImageDescription"},
{271,"Make"},
{272,"Model"},
{273,"StripOffsets"},
{274,"Orientation"},
{277,"SamplesPerPixel"},
{278,"RowsPerStrip"},
{279,"StripByteCounts"},
{280,"MinSampleValue"},
{281,"MaxSampleValue"},
{282,"XResolution"},
{283,"YResolution"},
{284,"PlanarConfiguration"},
{288,"FreeOffsets"},
{289,"FreeByteCounts"},
{290,"GrayResponseUnit"},
{291,"GrayResponseCurve"},
{296,"ResolutionUnit"},
{305,"Software"},
{306,"DateTime"},
{315,"Artist"},
{316,"HostComputer"},
{320,"ColorMap"},
{338,"ExtraSamples"},
{33432,"Copyright"},
{269,"DocumentName"},
{285,"PageName"},
{286,"XPosition"},
{287,"YPosition"},
{292,"T4Options"},
{293,"T6Options"},
{297,"PageNumber"},
{301,"TransferFunction"},
{317,"Predictor"},
{318,"WhitePoint"},
{319,"PrimaryChromaticities"},
{321,"HalftoneHints"},
{322,"TileWidth"},
{323,"TileLength"},
{324,"TileOffsets"},
{325,"TileByteCounts"},
{326,"BadFaxLines"},
{327,"CleanFaxData"},
{328,"ConsecutiveBadFaxLines"},
{330,"SubIFDs"},
{332,"InkSet"},
{333,"InkNames"},
{334,"NumberOfInks"},
{336,"DotRange"},
{337,"TargetPrinter"},
{339,"SampleFormat"},
{340,"SMinSampleValue"},
{341,"SMaxSampleValue"},
{342,"TransferRange"},
{343,"ClipPath"},
{344,"XClipPathUnits"},
{345,"YClipPathUnits"},
{346,"Indexed"},
{347,"JPEGTables"},
{351,"OPIProxy"},
{400,"GlobalParametersIFD"},
{401,"ProfileType"},
{402,"FaxProfile"},
{403,"CodingMethods"},
{404,"VersionYear"},
{405,"ModeNumber"},
{433,"Decode"},
{434,"DefaultImageColor"},
{512,"JPEGProc"},
{513,"JPEGInterchangeFormat"},
{514,"JPEGInterchangeFormatLength"},
{515,"JPEGRestartInterval"},
{517,"JPEGLosslessPredictors"},
{518,"JPEGPointTransforms"},
{519,"JPEGQTables"},
{520,"JPEGDCTables"},
{521,"JPEGACTables"},
{529,"YCbCrCoefficients"},
{530,"YCbCrSubSampling"},
{531,"YCbCrPositioning"},
{532,"ReferenceBlackWhite"},
{559,"StripRowCounts"},
{700,"XMP"},
{32781,"ImageID"},
{34732,"ImageLayer"}
        };



            public static Dictionary<int, string> TagValue
            {
                get
                {
                    return _value;

                }

            }

        }

    }
