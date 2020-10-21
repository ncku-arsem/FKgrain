#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <time.h>

int main()
{
 char path[] = "E:/GravelFK";
 char FKfilename[1024];
 int num = 49;
 sprintf(FKfilename, "%s/S4-RP/HO4FK/FKho4S4RPdtNZdsm-%d.out", path, num);
 
 double x, y, z, true, noise, local, reg;
  
 FILE *fp = fopen(FKfilename, "rt");
 
 char localFilename[1024];
 char regFilename[1024];
 sprintf(localFilename, "%s/S4-RP/HO4FK/com/HO4S4RPvgmlocal-%d.txt", path, num);
 sprintf(regFilename, "%s/S4-RP/HO4FK/com/HO4S4RPvgmreg-%d.txt", path, num);
 
 FILE *ofp1 = fopen(localFilename, "wt");
 FILE *ofp2 = fopen(regFilename, "wt");
 
 int index = 0;
 while( fscanf(fp, "%lf %lf %lf %lf %lf %lf", &x, &y, &z, &noise, &local, &reg) == 6)
 {
   fprintf(ofp1, "%.3f %.3f %.6f\n", x, y, local);
   fprintf(ofp2, "%.3f %.3f %.6f\n", x, y, reg);
 }
 fclose(fp);
 fclose(ofp1);
 fclose(ofp2);
  
 return 0;
}