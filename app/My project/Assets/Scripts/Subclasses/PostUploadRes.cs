class PostUploadRes
{
    public int id { get; set; }
    public string filename { get; set; }
    public PostUploadRes(int sightingId, string sightingFilename)
    {
        id = sightingId;
        filename = sightingFilename;
    }
}

