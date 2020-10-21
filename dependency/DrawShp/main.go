package main

import (
	"bufio"
	"flag"
	"fmt"
	"log"
	"math"
	"os"
	"strconv"
	"strings"

	"github.com/jonas-p/go-shp"
)

type worldFile struct {
	Xsize float64
	Ysize float64
	TLx   float64
	TLy   float64
}

func main() {
	tfw := flag.String("tfw", "", "world file")
	eclipse := flag.String("eclipse", "", "eclipse file")
	boundaries := flag.String("boundaries", "", "boundaries file")
	shpfile := flag.String("o", "output", "save as, will have o.shp and o_eclips.shp")
	flag.Parse()
	if *eclipse == "" || *boundaries == "" || *tfw == "" {
		return
	}
	world := worldFile{Xsize: 1, Ysize: -1, TLx: 0, TLy: 0}
	tfwfile, err := os.Open(*tfw)
	if err == nil {
		tfwscanner := bufio.NewScanner(tfwfile)
		k := 0
		for tfwscanner.Scan() {
			s := tfwscanner.Text()
			tmp, err := strconv.ParseFloat(s, 64)
			if err != nil {
				fmt.Println("Fail to parse world file")
				return
			}
			switch k {
			case 0:
				world.Xsize = tmp
			case 3:
				world.Ysize = tmp
			case 4:
				world.TLx = tmp
			case 5:
				world.TLy = tmp
			}
			k++
		}
	}
	tfwfile.Close()
	eclipsefile, err := os.Open(*eclipse)
	if err != nil {
		log.Println(err)
	}

	scanner := bufio.NewScanner(eclipsefile)
	ecli := make([][]float64, 0)
	// read eclipse into array
	for scanner.Scan() {
		ar := strings.Split(scanner.Text(), ",")
		x, _ := strconv.ParseFloat(ar[2], 64)
		y, _ := strconv.ParseFloat(ar[3], 64)
		x, y = world.Img2WorldCoordinate(x, y)
		a, _ := strconv.ParseFloat(ar[0], 64)
		b, _ := strconv.ParseFloat(ar[1], 64)
		tilt, _ := strconv.ParseFloat(ar[4], 64)
		if b > a {
			tmp := a
			a = b
			b = tmp
			tilt += 90
		}

		a = world.Pixel2Distance(a)
		b = world.Pixel2Distance(b)
		e := []float64{x, y, a, b, tilt}
		ecli = append(ecli, e)
	}
	eclipsefile.Close()

	// read in boundary
	boundariesfile, err := os.Open(*boundaries)
	if err != nil {
		log.Println(err)
	}
	bscanner := bufio.NewScanner(boundariesfile)
	bscanner.Scan()
	nOfBound, _ := strconv.Atoi(bscanner.Text())
	boundSizes := make([]int, nOfBound)
	for i := 0; i < nOfBound; i++ {
		bscanner.Scan()
		s, _ := strconv.Atoi(bscanner.Text())
		boundSizes[i] = s
	}
	boundariesData := make([][]shp.Point, nOfBound)
	for k, siz := range boundSizes {
		bd := make([]shp.Point, siz)
		for i := 0; i < siz; i++ {
			bscanner.Scan()
			ar := strings.Split(bscanner.Text(), ",")
			x, _ := strconv.ParseFloat(ar[0], 64)
			y, _ := strconv.ParseFloat(ar[1], 64)
			x, y = world.Img2WorldCoordinate(x, y)
			p := shp.Point{X: x, Y: y}
			bd[i] = p
		}
		boundariesData[k] = bd
	}
	// draw boundaryShp
	drawBoundariesShp(*shpfile, boundariesData, ecli)
	// draw eclipseShp
	drawEllipseShp(*shpfile+"_ellipse", ecli, 360)
}

func drawBoundariesShp(filename string, boundariesData [][]shp.Point, ellipseData [][]float64) {
	boundaryShp, err := shp.Create(filename, shp.POLYGON)
	if err != nil {
		panic(err)
	}
	defer boundaryShp.Close()
	fields := []shp.Field{
		// String attribute field with length 40
		shp.NumberField("ID", 8),
		shp.FloatField("x", 30, 4),
		shp.FloatField("y", 30, 4),
		shp.FloatField("a", 30, 16),
		shp.FloatField("b", 30, 16),
		shp.FloatField("tilt", 30, 4),
	}
	boundaryShp.SetFields(fields)

	for i, el := range ellipseData {
		polygons := shp.NewPolyLine([][]shp.Point{boundariesData[i]})
		boundaryShp.Write(polygons)
		boundaryShp.WriteAttribute(i, 0, i+1)
		err = boundaryShp.WriteAttribute(i, 1, el[0])
		if err != nil {
			fmt.Println(err)
		}
		err = boundaryShp.WriteAttribute(i, 2, el[1])
		if err != nil {
			fmt.Println(err)
		}
		err = boundaryShp.WriteAttribute(i, 3, el[2]*2)
		if err != nil {
			fmt.Println(err)
		}
		err = boundaryShp.WriteAttribute(i, 4, el[3]*2)
		if err != nil {
			fmt.Println(err)
		}
		err = boundaryShp.WriteAttribute(i, 5, el[4])
		if err != nil {
			fmt.Println(err)
		}

	}
}

func drawEllipseShp(filename string, ellipseData [][]float64, NumberOfPoints int) {
	ellipseShp, err := shp.Create(filename, shp.POLYGON)
	if err != nil {
		panic(err)
	}
	defer ellipseShp.Close()

	fields := []shp.Field{
		// String attribute field with length 40
		shp.NumberField("ID", 8),
		shp.FloatField("x", 30, 4),
		shp.FloatField("y", 30, 4),
		shp.FloatField("a", 30, 16),
		shp.FloatField("b", 30, 16),
		shp.FloatField("tilt", 30, 4),
	}
	ellipseShp.SetFields(fields)
	for i, el := range ellipseData {
		polygonPoints := drawEclipse(el[0], el[1], el[2], el[3], el[4]/180*math.Pi, NumberOfPoints)
		polygons := shp.NewPolyLine([][]shp.Point{polygonPoints})
		ellipseShp.Write(polygons)
		ellipseShp.WriteAttribute(i, 0, i+1)
		err = ellipseShp.WriteAttribute(i, 1, el[0])
		if err != nil {
			fmt.Println(err)
		}
		err = ellipseShp.WriteAttribute(i, 2, el[1])
		if err != nil {
			fmt.Println(err)
		}
		err = ellipseShp.WriteAttribute(i, 3, el[2]*2)
		if err != nil {
			fmt.Println(err)
		}
		err = ellipseShp.WriteAttribute(i, 4, el[3]*2)
		if err != nil {
			fmt.Println(err)
		}
		err = ellipseShp.WriteAttribute(i, 5, el[4])
		if err != nil {
			fmt.Println(err)
		}

	}
}

func drawEclipse(dx, dy, a, b, tilt float64, NumberOfPoints int) []shp.Point {
	dt := 2 * math.Pi / float64(NumberOfPoints)
	tilt = 2*math.Pi - tilt
	theta := float64(0)
	coor := make([]shp.Point, NumberOfPoints)
	for i := 0; i < NumberOfPoints; i++ {
		x := a * math.Cos(theta)
		y := b * math.Sin(theta)
		xp := (x * math.Cos(tilt)) - (y * math.Sin(tilt))
		yp := (y * math.Cos(tilt)) + (x * math.Sin(tilt))
		xp += dx
		yp += dy
		theta += dt
		coor[i] = shp.Point{X: xp, Y: yp}
	}
	return coor
}

func (world *worldFile) Img2WorldCoordinate(x, y float64) (float64, float64) {
	var gx, gy float64
	gx = ((float64(x)) * world.Xsize) + world.TLx
	gy = ((float64(y)) * world.Ysize) + (world.TLy)
	return gx, gy
}
func (world *worldFile) Pixel2Distance(x float64) float64 {
	var d float64
	d = float64(x) * math.Abs(world.Xsize)
	return d
}
