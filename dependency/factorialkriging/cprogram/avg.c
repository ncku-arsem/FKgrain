#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <time.h>

char path[] = "E:/GravelFK/S4-RP";

int main()
{
 double x, y, z;
 int classification;
 int count[6];
 
 char filename[512];
 char outfilename[512];
 
 int index = 0;
 int inner = 0;
 double **sum = (double**)malloc(600 * sizeof(double*));
 for(index = 0; index < 600; index++)
   sum[index] = (double*)malloc(600 * sizeof(double));
 for(index = 0; index < 600; index++)
 {
  for(inner = 0; inner < 600; inner++)
   sum[index][inner] = 0.0;
 }
  
 double **div = (double**)malloc(600 * sizeof(double*));
 for(index = 0; index < 600; index++)
   div[index] = (double*)malloc(600 * sizeof(double));
 for(index = 0; index < 600; index++)
 {
  for(inner = 0; inner < 600; inner++)
   div[index][inner] = 0.0;
 }
 
 sprintf(filename, "%s/las/S6-edit.txt", path);
 puts(filename);
 
 count[0] = 0;
 count[1] = 0;
 count[2] = 0;
 count[3] = 0;
 count[4] = 0;
 count[5] = 0;
 
 double maxx = -9999.0;
 double maxy = -9999.0;
 
 double minx = 9999.0;
 double miny = 9999.0;
 
 double maxz = -9999.0;
 double minz = 9999.0;
  
 int sx, sy;	
 FILE *fp = fopen(filename, "rt");
 while(fscanf(fp, "%lf %lf %lf %d", &x, &y, &z, &classification) == 4)
 {
  if(classification == 1)
   count[1] += 1;
  if(classification == 2)
   count[2] += 1;
  if(classification == 6)
   count[3] += 1;
  if(classification == 7)
   count[4] += 1;
  if(classification == 8)
   count[5] += 1;
   
 /*  if(x > 0 && x < 0.01 && y > 0.28 && y < 0.29)
   {     sx = (floor)((x + 0.000001) / 0.01);
     sy = (floor)((y + 0.000001) / 0.01);
    printf("%.5f %.5f %.5f %d %d\n ", x, y, z, sx, sy);
	}*/
  if(classification == 1 || classification == 2 || classification == 8)
  {
   if(x < 6.0)
   {
    if(y < 6.0)
	{
     sx = (floor)((x + 0.00000) / 0.01);
     sy = (floor)((y + 0.00000) / 0.01);
	 
	 if(sx == 600)
      printf("x: %.6f\n", x);
     if(sy == 600)
      printf("y: %.6f\n", y);
	 
 	 sum[sx][sy] += z;
     div[sx][sy] += 1.0;
     
     if(x > maxx)
      maxx = x;
     if(y > maxy)
      maxy = y;
     if(z > maxz)
      maxz = z;		  
     if(x < minx)
      minx = x;
     if(y < miny)
      miny = y;	
     if(z < minz)
      minz = z;		  
	}
   }
  }
  count[0] += 1;
 }
 
 fclose(fp);
 printf("%d %d %d %d %d %d\n", count[0], count[1], count[2], count[3], count[4], count[5]);    
 printf("%.5f %.5f\n", maxx, minx);
 printf("%.5f %.5f\n", maxy, miny);
 printf("%.5f %.5f\n", maxz, minz);
 
 sprintf(outfilename, "%s/las/S4filterDSM.txt", path);
 puts(outfilename);
 FILE *ofp = fopen(outfilename, "wt");
 
 x = 0.0;
 for(index = 0; index < 600; index++)
 {
  y = 0.0;
  for(inner = 0; inner < 600; inner++)
  {
   if( div[index][inner] > 0)
   {
    z = sum[index][inner] / div[index][inner];
    fprintf(ofp, "%.5f %.5f %.5f\n", x, y, z);
   }
   y += 0.01;
  }
  x += 0.01;
 }
 fclose(ofp);
 
 return 0;
}
 