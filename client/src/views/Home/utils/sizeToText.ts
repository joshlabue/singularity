const sizeToText = (size: number) => {
    if(size < 10e8) { // if smaller than a gig
        const megabytes = Math.round(size / 1000000);
        return megabytes + " MB"
    }
    else {
        const gigabytes = Math.round(size / 10e7) / 10;
        return gigabytes + " GB"
    }
}

export default sizeToText;