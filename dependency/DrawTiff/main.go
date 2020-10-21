package main

import (
	"bufio"
	"flag"
	"fmt"
	"image"
	"io"
	"log"
	"math"
	"os"
	"strings"

	tiff32 "github.com/hongping1224/go-tiff32"
)

const suffix = "_FactorialResult.out"
const shortSuf = "_FK_Short_Range"
const longSuf = "_FK_Long_Range"
const combineSuf = "_FK_Combine"

func main() {

	fk := flag.String("fk", "", "world file")
	dem := flag.String("dem", "", "eclipse file")
	detrenddem := flag.String("detrenddem", "", "boundaries file")
	flag.Parse()
	if *fk == "" || *dem == "" || *detrenddem == "" {
		return
	}
	facpath := *fk
	dempath := *dem
	detrenddempath := *detrenddem

	//read in DEM
	readDEM(dempath)
	readDEM(detrenddempath)
	//read in detrendDEM

	//read in factorial
	readFactorialResult(facpath)
}

func generateTFW(src, dst string) error {
	sourceFileStat, err := os.Stat(src)
	if err != nil {
		return err
	}

	if !sourceFileStat.Mode().IsRegular() {
		return fmt.Errorf("%s is not a regular file.", src)
	}

	source, err := os.Open(src)
	if err != nil {
		return err
	}
	defer source.Close()

	destination, err := os.Create(dst)
	if err != nil {
		return err
	}
	buf := make([]byte, 64)
	for {
		n, err := source.Read(buf)
		if err != nil && err != io.EOF {
			return err
		}
		if n == 0 {
			break
		}

		if _, err := destination.Write(buf[:n]); err != nil {
			return err
		}
	}
	destination.Close()

	return nil
}

func readDEM(path string) {
	file, err := os.Open(path)
	if err != nil {
		log.Fatal(err)
	}
	defer file.Close()
	scanner := bufio.NewScanner(file)
	data := make([][3]float64, 0)
	minx, miny, maxx, maxy := math.Inf(1), math.Inf(1), math.Inf(-1), math.Inf(-1)
	for scanner.Scan() {
		var x, y, z float64
		fmt.Sscanf(scanner.Text(), "%f %f %f", &x, &y, &z)
		data = append(data, [3]float64{x, y, z})
		minx = math.Min(x, minx)
		miny = math.Min(y, miny)
		maxx = math.Max(x, maxx)
		maxy = math.Max(y, maxy)
	}
	diff := math.Inf(1)
	for i := 1; i < len(data); i++ {
		dx := math.Abs(data[i][0] - data[i-1][0])
		dy := math.Abs(data[i][1] - data[i-1][1])
		if dx != 0 {
			diff = math.Min(diff, dx)
		}
		if dy != 0 {
			diff = math.Min(diff, dy)
		}
	}
	diff = math.Round(diff*10000) / 10000
	sx := minx
	sy := miny
	ex := maxx
	ey := maxy
	xsize := (int)(math.Floor((ex-sx)/diff)) + 1
	ysize := (int)(math.Floor((ey-sy)/diff)) + 1
	imdata := make([][]float64, xsize)
	for i := 0; i < xsize; i++ {
		imdata[i] = make([]float64, ysize)
	}
	for i := 0; i < len(data); i++ {
		xindex := int(math.Round((data[i][0] - sx) / diff))
		yindex := int(math.Round((data[i][1] - sy) / diff))
		imdata[xindex][yindex] = data[i][2]
	}
	savetiff(strings.Replace(path, ".txt", ".tif", -1), xsize, ysize, imdata)
	writeTFW(strings.Replace(path, ".txt", ".tfw", -1), maxx, maxy, minx, miny, xsize, ysize)
}

func writeTFW(path string, maxx, maxy, minx, miny float64, xsize, ysize int) {
	f, err := os.Create(path)
	if err != nil {
		return
	}
	x := (maxx - minx) / float64(xsize)
	f.WriteString(fmt.Sprintf("%f\n", x))
	f.WriteString("0\n")
	f.WriteString("0\n")
	f.WriteString(fmt.Sprintf("-%f\n", x))
	f.WriteString(fmt.Sprintf("%f\n", minx+(x/2)))
	f.WriteString(fmt.Sprintf("%f\n", maxy-(x/2)))
	f.Close()
}

func readFactorialResult(path string) {
	file, err := os.Open(path)
	if err != nil {
		log.Fatal(err)
	}
	defer file.Close()
	scanner := bufio.NewScanner(file)
	//remove header
	for i := 0; i < 8; i++ {
		scanner.Scan()
	}

	data := make([][4]float64, 0)
	/*localmax, localmin := math.Inf(-1), math.Inf(1)
	regionmax, regionmin := math.Inf(-1), math.Inf(1)
	combinemax, combinemin := math.Inf(-1), math.Inf(1)*/
	for scanner.Scan() {
		var x, y, short, long float64
		var dump float64
		fmt.Sscanf(scanner.Text(), "%f %f %f %f %f %f", &x, &y, &dump, &dump, &short, &long)
		data = append(data, [4]float64{x, y, short, long})
		/*localmax = math.Max(localmax, short)
		localmin = math.Min(localmin, short)
		regionmax = math.Max(regionmax, long)
		regionmin = math.Min(regionmin, long)
		combinemax = math.Max(combinemax, long+short)
		combinemin = math.Min(combinemin, long+short)*/
	}
	//first data
	sx := data[0][0]
	sy := data[0][1]
	ex := data[len(data)-1][0]
	ey := data[len(data)-1][1]
	diff := math.Abs(ex - data[len(data)-2][0])
	diff = math.Round(diff*10000) / 10000
	xsize := (int)(math.Floor((ex-sx)/diff)) + 1
	ysize := (int)(math.Floor((ey-sy)/diff)) + 1
	Local := make([][]float64, xsize)
	Region := make([][]float64, xsize)
	Combine := make([][]float64, xsize)
	check := make([][]bool, xsize)
	for i := 0; i < xsize; i++ {
		Local[i] = make([]float64, ysize)
		Region[i] = make([]float64, ysize)
		Combine[i] = make([]float64, ysize)
		check[i] = make([]bool, ysize)
	}

	for i := 0; i < len(data); i++ {
		xindex := int(math.Round((data[i][0] - sx) / diff))
		if xindex == 29 {
			fmt.Println("2929292")
		}
		yindex := int(math.Round((data[i][1] - sy) / diff))
		Local[xindex][yindex] = data[i][2]
		Region[xindex][yindex] = data[i][3]
		Combine[xindex][yindex] = data[i][2] + data[i][3]
		check[xindex][yindex] = true
	}
	fmt.Println((data[29][0] - sx) / diff)
	/*for i := 0; i < xsize; i++ {
		if check[i][0] == false {
			fmt.Println("i")
			fmt.Println(i)
			fmt.Println("x")
			fmt.Println((float64(i) * diff) + sx)
			fmt.Println(Region[i][0])
		}
	}*/
	savetiff(strings.Replace(path, suffix, shortSuf+".tif", -1), xsize, ysize, Local)
	writeTFW(strings.Replace(strings.Replace(path, suffix, shortSuf+".tif", -1), ".tif", ".tfw", -1), ex, ey, sx, sy, xsize, ysize)
	savetiff(strings.Replace(path, suffix, longSuf+".tif", -1), xsize, ysize, Region)
	writeTFW(strings.Replace(strings.Replace(path, suffix, longSuf+".tif", -1), ".tif", ".tfw", -1), ex, ey, sx, sy, xsize, ysize)
	savetiff(strings.Replace(path, suffix, combineSuf+".tif", -1), xsize, ysize, Combine)
	writeTFW(strings.Replace(strings.Replace(path, suffix, combineSuf+".tif", -1), ".tif", ".tfw", -1), ex, ey, sx, sy, xsize, ysize)
}

func savetiff(path string, xsize, ysize int, data [][]float64) {
	im := tiff32.NewGrayFloat32(image.Rectangle{Min: image.Point{X: 0, Y: 0}, Max: image.Point{X: xsize, Y: ysize}})
	for y := 0; y < ysize; y++ {
		for x := 0; x < xsize; x++ {
			im.SetGray32(x, ysize-y-1, tiff32.GrayFloat32Color{Y: tiff32.FloatToUint(float32(data[x][y]))})
		}
	}
	f, err := os.Create(path)
	if err != nil {
		log.Fatal(err)
		return
	}
	err = tiff32.Encode(f, im, nil)
	if err != nil {
		log.Fatal(err)
	}
	f.Close()
}
