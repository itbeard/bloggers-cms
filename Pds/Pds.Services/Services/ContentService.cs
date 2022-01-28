﻿using AutoMapper;
using Pds.Core.Enums;
using Pds.Core.Exceptions.Content;
using Pds.Data;
using Pds.Data.Entities;
using Pds.Services.Interfaces;
using Pds.Services.Models.Content;

namespace Pds.Services.Services;

public class ContentService : IContentService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public ContentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<List<Content>> GetAllAsync()
    {
        return await unitOfWork.Content.GetAllFullAsync();
    }

    public async Task<Content> GetAsync(Guid contentId)
    {
        return await unitOfWork.Content.GetFullByIdAsync(contentId);
    }

    public async Task<Guid> CreateAsync(CreateContentModel model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        var content = new Content
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Title = model.Title,
            Status = ContentStatus.Active,
            Type = model.Type,
            BrandId = model.BrandId,
            SocialMediaType = model.SocialMediaType,
            Comment = model.Comment,
            ReleaseDate = model.ReleaseDate.Date,
            EndDate = model.EndDate?.Date,
            PersonId = model.PersonId
        };

        if (!model.IsFree)
        {
            var bill = new Bill
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Value = model.Bill.Value,
                ContentId = content.Id,
                Contact = model.Bill.Contact.Replace("@", string.Empty),
                ContactName = model.Bill.ContactName,
                ContactType = model.Bill.ContactType,
                PaymentStatus = model.Bill.Value == 0 ? PaymentStatus.Paid : PaymentStatus.NotPaid,
                Type = BillType.Content,
                BrandId = model.BrandId,
                ClientId = model.Bill.ClientId,
                Comment = $"Created automatically  for content with id {content.Id} ({content.Title})"
            };

            content.Bill = bill;
            content.BillId = bill.Id;
        }

        var result = await unitOfWork.Content.InsertAsync(content);

        return result.Id;
    }

    public async Task<Guid> EditAsync(EditContentModel model)
    {
        if (model == null)
        {
            throw new ContentEditException($"Модель запроса пуста.");
        }

        var content = await unitOfWork.Content.GetByIdWithBillAsync(model.Id);
            
        if (content == null)
        {
            throw new ContentEditException($"Контент с id {model.Id} не найден.");
        }

        if (content.Status == ContentStatus.Archived)
        {
            throw new ContentEditException($"Нельзя редактировать архивный контент.");
        }

        content.UpdatedAt = DateTime.UtcNow;
        content.Title = model.Title;
        content.Type = model.Type;
        content.SocialMediaType = model.SocialMediaType;
        content.Comment = model.Comment;
        content.ReleaseDate = model.ReleaseDate.Date;
        content.EndDate = model.EndDate?.Date;
        content.PersonId = model.PersonId != null && model.PersonId.Value == Guid.Empty ? null : model.PersonId;

        if (model.Bill != null && content.Bill != null) // Just update existed bill
        {
            content.Bill.ClientId =  model.Bill.ClientId;
            content.Bill.Contact =  model.Bill.Contact.Replace("@", string.Empty);
            content.Bill.ContactName =  model.Bill.ContactName;
            content.Bill.ContactType =  model.Bill.ContactType;
            content.Bill.Value =  model.Bill.Value;
            content.Bill.UpdatedAt = DateTime.UtcNow;
        }
        else if(model.Bill != null && content.Bill == null) // Create new bill
        {
            content.Bill = mapper.Map<Bill>(model.Bill);
            content.Bill.Id = Guid.NewGuid();
            content.Bill.Type = BillType.Content;
            content.Bill.BrandId = content.BrandId;
            content.Bill.CreatedAt = DateTime.UtcNow;
        }
        else if(model.Bill == null && content.Bill != null) // Delete bill
        {
            content.Bill = null;
        }

        var result = await unitOfWork.Content.FullUpdateAsync(content);

        return result.Id;
    }

    public async Task DeleteAsync(Guid clientId)
    {
        var content = await unitOfWork.Content.GetByIdWithBillWithCostsAsync(clientId);
        var bill = content?.BillId != null ? 
            await unitOfWork.Bills.GetFirstWhereAsync(b => b.Id == content.BillId) : 
            null;

        if (content == null)
        {
            throw new ContentDeleteException("Контент не найден.");
        }

        if (content.Status == ContentStatus.Archived)
        {
            throw new ContentDeleteException("Нельзя удалить заархивированый контент.");
        }

        await unitOfWork.Content.FullDeleteAsync(content);
    }

    public async Task ArchiveAsync(Guid contentId)
    {
        var content = await unitOfWork.Content.GetFullByIdAsync(contentId);
        if (content is {Status: ContentStatus.Active} && (content.Bill == null || content.Bill.PaymentStatus == PaymentStatus.Paid))
        {
            await unitOfWork.Content.FullArchiveAsync(content);
        }
    }

    public async Task UnarchiveAsync(Guid contentId)
    {
        var content = await unitOfWork.Content.GetFullByIdAsync(contentId);
        if (content is {Status: ContentStatus.Archived})
        {
            content.Status = ContentStatus.Active;
            content.UpdatedAt = DateTime.UtcNow;
            await unitOfWork.Content.UpdateAsync(content);
        }
    }
    
    public async Task<List<Content>> GetContentsForListByBrandId(Guid brandId)
    {
        var contents = new List<Content> {new() {Id = Guid.Empty}}; //Add empty as a first element of list
        contents.AddRange(await unitOfWork.Content.GetContentsForListByBrandId(brandId));

        return contents;
    }
    
    public async Task<List<Content>> GetContentsForListByBrandIdWithSelectedValue(Guid brandId, Guid? selectedContentId)
    {
        var initialContents = await GetContentsForListByBrandId(brandId);
        if (selectedContentId == null || initialContents == null) return initialContents;  
        
        // Add first content on top of the list if it possible
        var firstContent = await unitOfWork.Content.GetFirstWhereAsync(c => c.Id == selectedContentId);
        initialContents.Remove(firstContent);
        initialContents = initialContents.Prepend(firstContent).ToList();

        return initialContents;
    }
}