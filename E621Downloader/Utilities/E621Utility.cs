using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace E621Downloader.Utilities
{
    public static class E621Utility
    {
        public static string GetPostsUrl(long startId)
        {
            string query = $"id:>={startId} order:id_asc";
            string urlEncodedQuery = WebUtility.UrlEncode(query);
            return $"https://e621.net/posts.json?tags={urlEncodedQuery}&page=1&limit=1000";
        }

        public static async Task<JObject[]> GetPosts(long startId)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "E621AutoTag/0.1 (by checkium)");
                string url = GetPostsUrl(startId);
                string jsonString = await client.GetStringAsync(url);

                dynamic posts = JObject.Parse(jsonString);
                JArray jsonArray = posts.posts;

                return jsonArray.Cast<JObject>().ToArray();
            }
        }

        public static JObject TranslatePostFormat(JObject e621Object)
        {
            JObject danbooruObject = JObject.Parse(e621Object.ToString());

            dynamic score = (JObject) e621Object.GetValue("score");
            danbooruObject.Remove("score");
            danbooruObject.Add("score", score.total);
            danbooruObject.Add("up_score", score.up);
            danbooruObject.Add("down_score", score.down);

            JArray sources = (JArray) e621Object.GetValue("sources");
            danbooruObject.Remove("sources");
            if (sources.Count > 0) danbooruObject.Add("source", sources[0]);

            dynamic file = (JObject) e621Object.GetValue("file");
            danbooruObject.Remove("file");
            danbooruObject.Add("md5", file.md5);
            danbooruObject.Add("image_width", file.width);
            danbooruObject.Add("image_height", file.height);
            danbooruObject.Add("file_ext", file.ext);
            danbooruObject.Add("file_url", file.url);
            danbooruObject.Add("file_size", file.size);

            dynamic sample = (JObject)e621Object.GetValue("sample");
            danbooruObject.Remove("sample");
            danbooruObject.Add("large_file_url", sample.url);
            dynamic preview = (JObject)e621Object.GetValue("preview");
            danbooruObject.Remove("preview");
            danbooruObject.Add("preview_file_url", preview.url);

            dynamic tags = (JObject) e621Object.GetValue("tags");
            danbooruObject.Remove("tags");
            JArray generalTags = (JArray) tags.general;
            JArray speciesTags = (JArray) tags.species;
            JArray metaTags = (JArray)tags.meta;
            List<String> tagList = new List<String>();
            List<String> generalTagList = new List<string>();
            List<String> speciesTagList = new List<string>();
            List<String> metaTagList = new List<string>();
            for (int i = 0; i < generalTags.Count; i++)
            {
                tagList.Add((String)generalTags[i]);
                generalTagList.Add((String) generalTags[i]);
            }
            for (int i = 0; i < speciesTags.Count; i++)
            {
                tagList.Add((String)speciesTags[i]);
                speciesTagList.Add((String)speciesTags[i]);
            }
            for (int i = 0; i < metaTags.Count; i++)
            {
                metaTagList.Add((String)metaTags[i]);
            }
            danbooruObject.Add("tag_string", string.Join(" ", tagList));
            danbooruObject.Add("tag_count", tagList.Count);
            danbooruObject.Add("tag_string_general", string.Join(" ", generalTagList));
            danbooruObject.Add("tag_count_general", generalTagList.Count);
            danbooruObject.Add("tag_string_species", string.Join(" ", speciesTagList));
            danbooruObject.Add("tag_count_species", speciesTagList.Count);
            danbooruObject.Add("tag_string_meta", string.Join(" ", metaTagList));
            danbooruObject.Add("tag_count_meta", metaTagList.Count);

            dynamic flags = (JObject)e621Object.GetValue("flags");
            danbooruObject.Remove("flags");
            danbooruObject.Add("is_pending", flags.pending);
            danbooruObject.Add("is_flagged", flags.flagged);
            danbooruObject.Add("is_deleted", flags.deleted);
            danbooruObject.Add("is_note_locked", flags.note_locked);
            danbooruObject.Add("is_rating_locked", flags.rating_locked);

            danbooruObject.Remove("locked_tags");
            danbooruObject.Remove("change_seq");
            danbooruObject.Remove("pools");
            danbooruObject.Remove("relationships");

            return danbooruObject;
        }
    }
}
