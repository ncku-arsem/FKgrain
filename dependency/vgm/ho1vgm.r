rm(list = ls())
library(gstat)
library(sp)
args = commandArgs(trailingOnly=TRUE)

tic <- Sys.time() 
{
  
  #filename = "F:/PersonalProject/Stone/UIForm/From/bin/x64/Debug/factorialkriging/DetrendingDSM.txt"
  filename = args[1]
  station = read.table(filename, header = FALSE, col.names = c("x", "y", "z"))
  siterows = nrow(station)					   
  
  spsite = station
  coordinates(spsite) = ~x + y
  print("computing variogram")
  site.vgm = gstat::variogram(z ~ 1, spsite, width = 0.01, cutoff = 3.0)
  vgmrows = nrow(site.vgm)
  print("Finish Compute variogram")
  #filename = "F:/PersonalProject/Stone/UIForm/From/bin/x64/Debug/vgm/ho1S4RPdtvgm1cm.txt"
  filename = args[2]
  fp = file(filename, "w")
  cat("np dist gamma\n", file = fp, sep = "")
  for(index in 1:vgmrows)
  {
    cat(site.vgm$np[index], " ", 
        formatC(site.vgm$dist[index], digits = 15, width = -1, format = "fg"), " ",
        formatC(site.vgm$gamma[index], digits = 15, width = -1, format = "fg"), "\n", 
        file = fp, sep = "")         		
    
  }	
  close(fp) 
  
}
toc <- Sys.time()
comp.time <- toc - tic
lznr.fit = fit.variogram(site.vgm, model = vgm(0.01, "Sph", 2.0,  add.to = vgm(0.01, "Sph", 1, nugget = 0.000001)))
lznr.fit
filename = args[3]
capture.output(lznr.fit, file = filename)


