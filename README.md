# DocWordCount
This is a tool for encoding sensitive documents so that the documents:
1) cannot be meaningfully recovered; and 
2) can be analyzed by bag of words tools, such as LIWC, LSA, and topic modeling.

To accomplish this, we use dictionary of common words in English do simple word counting,
recording the word counts for each dictionary entry that occurs in a document. 
Each document is converted to a frequence count matrix. 
This kind of representation is called monogram frequency count, 
which is the hardest for document reconstruction. 
Current technology can only reconstruct to some degree from bigram count 
(see http://minds.wisconsin.edu/handle/1793/60654). 
This representation will allow us to conduct analyses 
that are insensitive to grammatical structure like 
sentence boundaries, punctuation, and syntactical structure. 
  
