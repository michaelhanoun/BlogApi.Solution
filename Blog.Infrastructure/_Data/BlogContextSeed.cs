using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Blog.Core.Entities;
using Blog.Core.Entities.Post_Aggregate;
using Microsoft.AspNetCore.Identity;

namespace Blog.Infrastructure._Data
{
    public static class BlogContextSeed
    {

        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser>userManager,BlogContext blogContext)
        {
            if (!roleManager.Roles.Any())
            {
                var role = new IdentityRole("Admin");
                await roleManager.CreateAsync(role); 
            }
            if (!userManager.Users.Any())
            {
                var user1 = new ApplicationUser() { UserName = "admin", Email = "admin@blog.com", Bio = "Super admin of the blog" };
                var user2 = new ApplicationUser() { UserName = "john_doe", Email = "john@example.com", Bio = "Tech enthusiast and writer" };
                
                await userManager.CreateAsync(user1, "Admin@123");
                await userManager.AddToRoleAsync(user1, "Admin");
               
                await userManager.CreateAsync(user2, "User@123"); 
            }
            if(!blogContext.Categories.Any())
            {
                var categoryData = File.ReadAllText("../Blog.Infrastructure/_Data/DataSeed/categories.json");
                var category = JsonSerializer.Deserialize<List<Category>>(categoryData);
                await blogContext.AddRangeAsync(category);
                await blogContext.SaveChangesAsync();
            }
            if (!blogContext.Posts.Any())
            {
                var user = await userManager.FindByEmailAsync("john@example.com");
                if (user is not null)
                {
                    var postData = File.ReadAllText("../Blog.Infrastructure/_Data/DataSeed/posts.json");
                    
                    var post = JsonSerializer.Deserialize<Post>(postData);
                    post.ApplicationUserId = user.Id;
                    await blogContext.AddAsync(post); 
                     await blogContext.SaveChangesAsync();
                }
            }
            if (!blogContext.Tags.Any())
            {
                    var tagsData = File.ReadAllText("../Blog.Infrastructure/_Data/DataSeed/tags.json");
                    var tags = JsonSerializer.Deserialize<List<Tag>>(tagsData);
                    await blogContext.AddRangeAsync(tags); 
            }

            if (!blogContext.PostCategories.Any()&&blogContext.Posts.FirstOrDefault(P=>P.Id==1)is not null && blogContext.Categories.FirstOrDefault(P => P.Id == 1) is not null && blogContext.Categories.FirstOrDefault(P => P.Id == 3) is not null)
            {
                var postCategoriesData = File.ReadAllText("../Blog.Infrastructure/_Data/DataSeed/postCategories.json");
                var postCategories = JsonSerializer.Deserialize<List<PostCategory>>(postCategoriesData);
                await blogContext.AddRangeAsync(postCategories); 
                await blogContext.SaveChangesAsync();
            }

            if (!blogContext.Comments.Any()&& blogContext.Posts.FirstOrDefault(P => P.Id == 1) is not null)
            {
                var user = await userManager.FindByEmailAsync("admin@blog.com");
                if (user is not null)
                {
                    var commentData = File.ReadAllText("../Blog.Infrastructure/_Data/DataSeed/comments.json");
                    var comment = JsonSerializer.Deserialize<Comment>(commentData);
                    comment.ApplicationUserId = user.Id;
                    await blogContext.AddAsync(comment);
                }
            }
            if (!blogContext.Likes.Any() && blogContext.Posts.FirstOrDefault(P => P.Id == 1) is not null)
            {
                var user = await userManager.FindByEmailAsync("admin@blog.com");
                if (user is not null)
                {
                    var likesData = File.ReadAllText("../Blog.Infrastructure/_Data/DataSeed/likes.json");
                    var likes = JsonSerializer.Deserialize<Like>(likesData);
                    likes.ApplicationUserId = user.Id;
                    await blogContext.AddAsync(likes);
                }
            }
            await blogContext.SaveChangesAsync();

        }
    }
}
