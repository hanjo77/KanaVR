for file in missing-otoya/*
do 
	sox $file $file silence 1 0.1 0.1% reverse silence 1 0.1 0.1% reverse
done
