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

func main() {
	mode := flag.String("m", "", "mode, b2csv , e2shp")
	path := flag.String("shp", "", "shapefile")
	outpath := flag.String("out", "", "outfile")
	flag.Parse()
	if *path == "" || *outpath == "" {
		return
	}
	switch *mode {
	case "b2csv":
		boundtocsv(*path, *outpath)
		break
	case "e2shp":
		ellipsetoshp(*path, *outpath)
		break
	}

}

func ellipsetoshp(path, outpath string) {
	eclipsefile, err := os.Open(path)
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
		a, _ := strconv.ParseFloat(ar[0], 64)
		b, _ := strconv.ParseFloat(ar[1], 64)
		tilt, _ := strconv.ParseFloat(ar[4], 64)
		if b > a {
			tmp := a
			a = b
			b = tmp
			tilt += 90
		}
		e := []float64{x, y, a, b, tilt}
		ecli = append(ecli, e)
	}
	eclipsefile.Close()
	drawEllipseShp(outpath, ecli, 360)
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

func boundtocsv(path, outpath string) {
	shape, err := shp.Open(path)
	if err != nil {
		log.Fatal(err)
	}
	defer shape.Close()
	out, err := os.Create(outpath)
	if err != nil {
		log.Fatal(err)
	}
	defer out.Close()

	switch shape.GeometryType {
	case shp.POLYGON:
		// loop through all features in the shapefile
		for shape.Next() {
			_, p := shape.Shape()
			s := p.(*shp.Polygon)
			n := int(s.NumPoints)
			out.Write([]byte(fmt.Sprintf("%d\n", n)))
			// print feature
			for i := 0; i < n; i++ {
				out.Write([]byte(fmt.Sprintf("%f,%f\n", s.Points[i].X, s.Points[i].Y)))
			}
		}
		break
	case shp.POLYLINE:
		for shape.Next() {
			_, p := shape.Shape()
			s := p.(*shp.PolyLine)
			n := int(s.NumPoints)
			out.Write([]byte(fmt.Sprintf("%d\n", n)))
			// print feature
			for i := 0; i < n; i++ {
				out.Write([]byte(fmt.Sprintf("%f,%f\n", s.Points[i].X, s.Points[i].Y)))
			}

		}
		break
	case shp.POLYGONZ:
		for shape.Next() {
			_, p := shape.Shape()
			s := p.(*shp.PolygonZ)
			n := int(s.NumPoints)
			out.Write([]byte(fmt.Sprintf("%d\n", n)))
			// print feature
			for i := 0; i < n; i++ {
				out.Write([]byte(fmt.Sprintf("%f,%f\n", s.Points[i].X, s.Points[i].Y)))
			}

		}
		break
	case shp.POLYLINEZ:
		for shape.Next() {
			_, p := shape.Shape()
			s := p.(*shp.PolyLineZ)
			n := int(s.NumPoints)
			out.Write([]byte(fmt.Sprintf("%d\n", n)))
			// print feature
			for i := 0; i < n; i++ {
				out.Write([]byte(fmt.Sprintf("%f,%f\n", s.Points[i].X, s.Points[i].Y)))
			}
		}
		break
	case shp.POLYGONM:
		for shape.Next() {
			_, p := shape.Shape()
			s := p.(*shp.PolygonM)
			n := int(s.NumPoints)
			out.Write([]byte(fmt.Sprintf("%d\n", n)))
			// print feature
			for i := 0; i < n; i++ {
				out.Write([]byte(fmt.Sprintf("%f,%f\n", s.Points[i].X, s.Points[i].Y)))
			}

		}
		break
	case shp.POLYLINEM:
		for shape.Next() {
			_, p := shape.Shape()
			s := p.(*shp.PolyLineM)
			n := int(s.NumPoints)
			out.Write([]byte(fmt.Sprintf("%d\n", n)))
			// print feature
			for i := 0; i < n; i++ {
				out.Write([]byte(fmt.Sprintf("%f,%f\n", s.Points[i].X, s.Points[i].Y)))
			}

		}
		break
	}
}
